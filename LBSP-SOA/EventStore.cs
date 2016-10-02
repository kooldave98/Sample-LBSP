using System;
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

        public IEnumerable<RawEvent> get_history()
        {
            return read_log(current_domain_stream, ReadDirection.Forward, 50)
                    .Where(e => e.Event.EventType.StartsWith(LoggedEventPointer))//Todo: standardise===>>> $"{LoggedEventPointer}-{log_payload.stream_name}"
                    .Select(e => resolve_pointer(Encoding.UTF8.GetString(e.Event.Data)));
        }

        public void Subscribe(string stream_name, Action<RawEvent> on_message_received)
        {
            var settings = new CatchUpSubscriptionSettings(50, 10, true, true);

            connection.SubscribeToStreamFrom(stream_name, get_last_position(stream_name), settings,
                (e, s) =>
                {
                    if (!s.Event.EventType.Contains(LoggedEventPointer))
                    {
                        on_message_received(new RawEvent(s.Event.EventId,
                                                            s.Event.Data,
                                                            s.Event.Metadata,
                                                            s.Event.EventType,
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

        public void PublishResponse(IEnumerable<RawEvent> raw_events, RawEventPointer log_payload = null)
        {
            var events = raw_events.Select(payload => new EventData(payload.id, payload.type, true, payload.data, payload.metadata));

            if (log_payload != null)
            {
                events =
                events.Union(new EventData(Guid.NewGuid(),
                                            $"{LoggedEventPointer}-{log_payload.stream_name}", //Todo: wrap this in a constant
                                            true,
                                            Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(log_payload)), null).ToEnumerable());
            }


            connection
                .AppendToStreamAsync(current_domain_stream,
                                    ExpectedVersion.Any,
                                    events)
                .Wait();
        }

        public void PublishErrors(IEnumerable<RawEvent> raw_events, string origin_stream)
        {
            var events = raw_events.Select(payload => new EventData(payload.id, payload.type, true, payload.data, payload.metadata));


            connection
                .AppendToStreamAsync($"{origin_stream}-Errors",
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
                    JsonConvert.DeserializeObject<RawEventPointer>(pointer_as_json)
                );

        }

        private RawEvent resolve_pointer(RawEventPointer pointer)
        {
            var task = connection.ReadEventAsync(pointer.stream_name, pointer.position, false);

            task.Wait();

            var raw = task.Result.Event.Value.Event;

            return new RawEvent(raw.EventId, raw.Data, raw.Metadata, raw.EventType);

        }

        //Todo: This would only work 
        //if (saved-last-processed-event-number <= count(published-events))
        private int get_last_position(string stream_name)
        {
            return
                read_log(current_domain_stream, ReadDirection.Backward, 2)
                    .First(e => e.Event.EventType == $"{LoggedEventPointer}-{stream_name}")
                    .Event.EventNumber;
        }




        public void unsubscribe_all()
        {
            connection.Close();
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
        }

        private readonly string current_domain_stream;
        private readonly IEventStoreConnection connection;
    }

}
