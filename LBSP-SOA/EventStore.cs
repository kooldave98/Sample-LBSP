using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using CodeStructures;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace LbspSOA
{
    public class GESEventStore : IEventStore
    {
        private const string LoggedEventPointer = "LoggedEventPointer";

        private const string ProcessedRequests = "ProcessedRequests";

        public IEnumerable<RawEvent> get_history()
        {
            return read_log(current_domain_stream, ReadDirection.Forward, 50)
                    .Where(e => e.Event.EventType.StartsWith(LoggedEventPointer))
                    .Select(e => resolve_pointer(e.Event.Data.ToJsonString()));
        }

        private ConcurrentDictionary<Guid, byte> processed_request_ids = new ConcurrentDictionary<Guid, byte>();

        public void Subscribe(string stream_name, Action<RecordedRawEvent> on_message_received)
        {
            var settings = new CatchUpSubscriptionSettings(50, 10, true, true);

            connection.SubscribeToStreamFrom(stream_name, StreamPosition.Start, settings,
                (e, s) =>
                {
                    if (!s.Event.EventType.Contains(LoggedEventPointer))
                    {
                        on_message_received(new RecordedRawEvent(new RawEvent(s.Event.EventId,
                                                                                s.Event.Data,
                                                                                s.Event.Metadata,
                                                                                s.Event.EventType),
                                                                    stream_name,
                                                                    s.Event.EventNumber));
                    }

                }, null,
                (ess, dr, ex) =>
                {
                    Console.WriteLine($"Subscription to {stream_name} dropped .. reconnecting");

                    Subscribe(stream_name, on_message_received);
                });
        }

        public void Publish(IEnumerable<RawEvent> raw_events)
        {
            var events = raw_events.Select(payload => new EventData(payload.id, payload.type, true, payload.data, payload.metadata));

            connection
                .AppendToStreamAsync(current_domain_stream,
                                    ExpectedVersion.Any,
                                    events)
                .Wait();
        }

        public void CommitAndPublish(RecordedRawEvent origin_event, IEnumerable<RawEvent> raw_events)
        {
            var log_payload = origin_event.ToPointer();


            //events to publish
            var events = raw_events.Select(payload => new EventData(payload.id, payload.type, true, payload.data, payload.metadata));

            //add origin request

            
            processed_request_ids.AddOrUpdate(origin_event.raw_event.id, default(byte), (g, b) => b);

            events =
                events.Union(new EventData(Guid.NewGuid(),
                                            $"{LoggedEventPointer}-{log_payload.stream_name}", //Todo: wrap this in a constant
                                            true,
                                            log_payload.ToBytes(),
                                            new { processed_request_ids }.ToBytes())
                                .ToEnumerable());


            //publish

            connection
                .AppendToStreamAsync(current_domain_stream,
                                    ExpectedVersion.Any,
                                    events)
                .Wait();
        }

        public void PublishErrors(RecordedRawEvent origin_event, IEnumerable<RawEvent> raw_events)
        {
            var events = raw_events.Select(payload => new EventData(payload.id, payload.type, true, payload.data, payload.metadata));


            connection
                .AppendToStreamAsync($"{origin_event.origin_stream}-Errors",
                                    ExpectedVersion.Any,
                                    events)
                .Wait();
        }

        private IEnumerable<ResolvedEvent> read_log(string stream_name, ReadDirection read_direction, int buffer)
        {
            StreamEventsSlice currentSlice;
            var nextSliceStart = StreamPosition.Start;

            do
            {
                if (read_direction == ReadDirection.Forward)
                {
                    currentSlice = connection
                                    .ReadStreamEventsForwardAsync(stream_name, nextSliceStart, buffer, false)
                                    .Result;
                }

                else
                {
                    currentSlice = connection
                                    .ReadStreamEventsBackwardAsync(stream_name, nextSliceStart, buffer, false)
                                    .Result;
                }

                nextSliceStart = currentSlice.NextEventNumber;

                foreach (var item in currentSlice.Events)
                {
                    yield return item;
                }

            } while (!currentSlice.IsEndOfStream);
        }

        private RawEvent resolve_pointer(string pointer_as_json)
        {
            return
                resolve_pointer(
                    JsonConvert.DeserializeObject<RecordedRawEventPointer>(pointer_as_json)
                );

        }

        private RawEvent resolve_pointer(RecordedRawEventPointer pointer)
        {
            var task = connection.ReadEventAsync(pointer.stream_name, pointer.position, false);

            task.Wait();

            var raw = task.Result.Event.Value.Event;

            return new RawEvent(raw.EventId, raw.Data, raw.Metadata, raw.EventType);

        }

        public void unsubscribe_all()
        {
            connection.Close();
        }

        private Dictionary<Guid, byte> get_latest_idempotency_values()
        {

            var raw_json = 
                read_log(current_domain_stream, ReadDirection.Backward, 2)
                    .Where(e => e.Event.EventType.StartsWith(LoggedEventPointer))
                    .Select(e => e.Event.Metadata.ToJsonString())
                    .FirstOrDefault();

            return 
                raw_json == null ? 
                new Dictionary<Guid, byte>() : 
                JsonConvert.DeserializeObject<Dictionary<Guid, byte>>(raw_json);
        }

        public GESEventStore(string permanent_world_name)
        {
            this.current_domain_stream = permanent_world_name;

            var backendSettings = ConnectionSettings.Create()
                //.UseConsoleLogger()
                .KeepReconnecting()
                .KeepRetrying();

            connection = EventStoreConnection.Create(backendSettings,
                new IPEndPoint(
                    //IPAddress.Parse("23.100.61.219"),
                    IPAddress.Loopback, //localhost
                    1113));

            connection.ConnectAsync().Wait();

            processed_request_ids = new ConcurrentDictionary<Guid, byte>(get_latest_idempotency_values());
        }

        private readonly string current_domain_stream;
        private readonly IEventStoreConnection connection;
    }

}
