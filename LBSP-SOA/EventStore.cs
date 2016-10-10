using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using CodeStructures;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace LbspSOA
{
    public class EventStore
    {
        private const string LoggedEventPointer = "LoggedEventPointer";

        private const string ProcessedRequests = "ProcessedRequests";

        public IEnumerable<RawEvent> get_history()
        {
            return read_log(current_domain_stream, ReadDirection.Forward, 50)
                    .Where(e => e.Event.EventType.StartsWith(LoggedEventPointer))
                    .Select(e => resolve_pointer(e.Event.Data.ToJsonString()));
        }

        private ConcurrentDictionary<Guid, byte> received_request_ids = new ConcurrentDictionary<Guid, byte>();
        private ConcurrentDictionary<Guid, byte> processed_request_ids = new ConcurrentDictionary<Guid, byte>();

        private Subject<RecordedRawEvent> combined_new_subject = new Subject<RecordedRawEvent>();
        public IObservable<RecordedRawEvent> NewEvents(params string[] stream_names)
        {
            foreach (var stream in stream_names)
            {
                NewEvents(stream, combined_new_subject);
            }

            return combined_new_subject.AsObservable();
        }
        public IObservable<RecordedRawEvent> NewEvents(string stream_name, Subject<RecordedRawEvent> new_subject = null)
        {
            new_subject = new_subject ?? new Subject<RecordedRawEvent>();

            connection.SubscribeToStreamAsync(stream_name, true,
                (e, s) =>
                {

                    var recorded_event = new RecordedRawEvent(new RawEvent(s.Event.EventId,
                                                                            s.Event.Data,
                                                                            s.Event.Metadata,
                                                                            s.Event.EventType),
                                                                stream_name,
                                                                s.Event.EventNumber);

                    new_subject.OnNext(recorded_event);

                },
                (ess, dr, ex) =>
                {
                    Console.WriteLine($"Subscription to {stream_name} dropped .. reconnecting");

                    NewEvents(stream_name, new_subject);
                });

            return new_subject.AsObservable();
        }


        private Subject<RecordedRawEvent> combined_all_unprocessed_subject = new Subject<RecordedRawEvent>();
        public IObservable<RecordedRawEvent> AllUnprocessedEvents(params string[] stream_names)
        {
            foreach (var stream in stream_names)
            {
                AllUnprocessedEvents(stream, combined_all_unprocessed_subject);
            }

            return combined_all_unprocessed_subject.AsObservable();
        }
        public IObservable<RecordedRawEvent> AllUnprocessedEvents(string stream_name, Subject<RecordedRawEvent> allUnprocessedSubject = null)
        {
            allUnprocessedSubject = allUnprocessedSubject ?? new Subject<RecordedRawEvent>();

            var settings = new CatchUpSubscriptionSettings(50, 10, true, true);

            connection.SubscribeToStreamFrom(stream_name, StreamCheckpoint.StreamStart, settings,
                (e, s) =>
                {
                    if (!received_request_ids.ContainsKey(s.Event.EventId)
                        &&
                        !processed_request_ids.ContainsKey(s.Event.EventId)
                        &&
                        !s.Event.EventType.Contains(LoggedEventPointer))
                    {
                        received_request_ids.AddOrUpdate(s.Event.EventId, default(byte), (g, b) => b);

                        var recorded_event =
                            new RecordedRawEvent(new RawEvent(s.Event.EventId,
                                                                s.Event.Data,
                                                                s.Event.Metadata,
                                                                s.Event.EventType),
                                                    stream_name,
                                                    s.Event.EventNumber);
                        

                        allUnprocessedSubject.OnNext(recorded_event);
                    }

                }, null,
                (ess, dr, ex) =>
                {
                    Console.WriteLine($"Subscription to {stream_name} dropped .. reconnecting");

                    AllUnprocessedEvents(stream_name, allUnprocessedSubject);
                });

            return allUnprocessedSubject.AsObservable();
        }

        public void Publish(params RawEvent[] raw_events)
        {
            var events = raw_events.Select(payload => new EventData(payload.id, payload.type, true, payload.data, payload.metadata));

            connection
                .AppendToStreamAsync(current_domain_stream,
                                    ExpectedVersion.Any,
                                    events)
                .Wait();
        }

        public void Publish(IEnumerable<RawEvent> raw_events)
        {
            Publish(raw_events.ToArray());
        }

        public void CommitAndPublish(RecordedRawEvent origin_event, IEnumerable<RawEvent> raw_events)
        {
            var log_payload = origin_event.ToPointer();


            //events to publish
            var events = raw_events.Select(payload => new EventData(payload.id, payload.type, true, payload.data, payload.metadata));


            processed_request_ids.AddOrUpdate(origin_event.raw_event.id, default(byte), (g, b) => b);
            //cleanup
            byte remove; received_request_ids.TryRemove(origin_event.raw_event.id, out remove);

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
            //Add to in-memory: will be made durable when the next request is persisted
            processed_request_ids.AddOrUpdate(origin_event.raw_event.id, default(byte), (g, b) => b);
            byte remove; received_request_ids.TryRemove(origin_event.raw_event.id, out remove);

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

            var nextSliceStart = read_direction == ReadDirection.Forward ? StreamPosition.Start : StreamPosition.End;

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

            return (
                raw_json == null ?
                new ProcessedRequestIdsWrapper() :
                JsonConvert.DeserializeObject<ProcessedRequestIdsWrapper>(raw_json)
                ).processed_request_ids;
        }

        public EventStore(string permanent_world_name)
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

    public class ProcessedRequestIdsWrapper
    {
        public Dictionary<Guid, byte> processed_request_ids { get; set; } = new Dictionary<Guid, byte>();
    }
    public class RecordedRawEvent
    {
        public readonly string origin_stream;
        public readonly int position_in_stream;
        public readonly RawEvent raw_event;

        public RecordedRawEvent(RawEvent raw_event,
                                string origin_stream,
                                int position_in_stream)
        {
            this.raw_event = raw_event;
            this.origin_stream = origin_stream;
            this.position_in_stream = position_in_stream;
        }

        public RecordedRawEventPointer ToPointer()
        {
            if (string.IsNullOrWhiteSpace(this.origin_stream)) throw new InvalidOperationException("origin stream is empty");
            if (this.position_in_stream < 0) throw new InvalidOperationException("position is invalid");


            return new RecordedRawEventPointer(this.origin_stream, this.position_in_stream);
        }
    }

    public static class RawEventExtensions
    {
        public static byte[] ToBytes(this object source)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(source));
        }

        public static string ToJsonString(this byte[] bytes_data)
        {
            return Encoding.UTF8.GetString(bytes_data);
        }

        public static T To<T>(this string json_string)
        {
            return JsonConvert.DeserializeObject<T>(json_string);
        }

        public static T To<T>(this byte[] bytes_data)
        {
            return To<T>(bytes_data.ToJsonString());
        }

        public static T ToAnonType<T>(this byte[] bytes_data, T t_object)
        {
            return ToAnonType(bytes_data.ToJsonString(), t_object);
        }

        public static T ToAnonType<T>(this string json_string, T t_object)
        {
            return JsonConvert.DeserializeAnonymousType(json_string, t_object);
        }

        public static object To(this string json_string, Type target_type)
        {            
            return JsonConvert.DeserializeObject(json_string, target_type);
        }

        public static object To(this byte[] bytes_data, Type target_type)
        {
            return To(bytes_data.ToJsonString(), target_type);
        }
    }


    public class RawEvent
    {
        public readonly Guid id;
        public readonly byte[] data;
        public readonly byte[] metadata;
        public readonly string type;

        public RawEvent(Guid id,
                        byte[] data,
                        byte[] metadata,
                        string type)
        {
            this.id = id;
            this.data = data;
            this.metadata = metadata;
            this.type = type;
        }
    }

    public class RecordedRawEventPointer
    {
        public string stream_name { get; set; }

        public int position { get; set; }

        public RecordedRawEventPointer(string stream_name, int position)
        {
            this.stream_name = stream_name;
            this.position = position;
        }
    }
}