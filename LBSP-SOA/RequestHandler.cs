using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LbspSOA
{
    public class RequestHandler<W> where W : IWorld
    {
        private void handle(RecordedRawEvent raw_event)
        {
            Console.WriteLine($"Received event with id: {raw_event.raw_event.id}");

            var request = to_raw_request(raw_event.raw_event);

            if (raw_event_requests.TryAdd(raw_event.raw_event.id, raw_event))
            {
                requests.Add(request);
            }
        }

        private RawRequest<W> to_raw_request(RawEvent raw_event)
        {
            var o = JToken.Parse(Encoding.UTF8.GetString(raw_event.data));
            return
            new RawRequest<W>(raw_event.id,
                        raw_event.data.ToJsonDynamic(),
                        raw_event.type,
                        router.get_handler(raw_event.type));
        }

        private ConcurrentDictionary<Guid, RecordedRawEvent> raw_event_requests = new ConcurrentDictionary<Guid, RecordedRawEvent>();

        public void start_listening(params string[] streams)
        {
            foreach (var stream in streams)
            {
                event_store.Subscribe(stream, handle);
            }
            

            Task.Run(() => {
                foreach (var response in responses.GetConsumingEnumerable())
                {
                    response
                        .core_response
                        .match(
                            is_success: events => {

                                var payload = 
                                    events.Select(ev=> new RawEvent(Guid.NewGuid()
                                            , ev.ToBytes()
                                            , new { parent_id = response.raw_request.id }.ToBytes()//Every event has a pointer to its parent event
                                            , ev.GetType().Name));

                                RecordedRawEvent input_raw_event;

                                if (raw_event_requests.TryRemove(response.raw_request.id, out input_raw_event))
                                {
                                    //Potential data inconsistencies between removing above, and publishing below
                                    event_store.PublishResponse(payload, input_raw_event);
                                }

                            },
                            is_error: errors => {

                                var payload =
                                    errors.Select(ev => new RawEvent(Guid.NewGuid()
                                            , ev.ToBytes()
                                            , new { parent_id = response.raw_request.id }.ToBytes()//Every event has a pointer to its parent event
                                            , ev.GetType().Name));

                                RecordedRawEvent input_raw_event;

                                if (raw_event_requests.TryRemove(response.raw_request.id, out input_raw_event))
                                {
                                    //Potential data inconsistencies between removing above, and publishing below
                                    event_store.PublishErrors(payload, input_raw_event.origin_stream);
                                }
                            }
                        );                    
                }

            });


            Console.WriteLine($"Started listening to event-store for events...");
        }

        public IEnumerable<RawRequest<W>> get_history()
        {
            return
                event_store
                .get_history()
                .Select(ev =>
                            new RawRequest<W>(ev.id,
                                            ev.data.ToJsonDynamic(),
                                            ev.type,
                                            router.get_handler(ev.type)));
        }

        public void stop_listening()
        {
            event_store.unsubscribe_all();
        }

        public RequestHandler(BlockingCollection<RawRequest<W>> the_requests
                            , BlockingCollection<RawResponse<W>> the_responses
                            , IEventStore the_event_store
                            , IRouter<W> the_router)
        {
            requests = the_requests;
            responses = the_responses;
            router = the_router;
            event_store = the_event_store;
        }

        private readonly BlockingCollection<RawRequest<W>> requests;
        private readonly BlockingCollection<RawResponse<W>> responses;
        private readonly IRouter<W> router;
        private readonly IEventStore event_store;
    }
}
