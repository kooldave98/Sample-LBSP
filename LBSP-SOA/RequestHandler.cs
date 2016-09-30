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
        private void handle(RawEvent raw_event)
        {
            Console.WriteLine($"Received event with id: {raw_event.id}");

            var request = to_raw_request(raw_event);

            if (raw_event_requests.TryAdd(raw_event.id, raw_event))
            {
                requests.Add(request);
            }
        }

        private RawRequest<W> to_raw_request(RawEvent raw_event)
        {
            return
            new RawRequest<W>(raw_event.id,
                        JToken.Parse(Encoding.UTF8.GetString(raw_event.data)),
                        raw_event.type,
                        router.get_handler(raw_event.type));
        }

        private ConcurrentDictionary<Guid, RawEvent> raw_event_requests = new ConcurrentDictionary<Guid, RawEvent>();

        public void start_listening()
        {
            event_store.Subscribe("PlannedSupply", "ShiftCalendarPublished", handle);

            Task.Run(() => {
                foreach (var response in responses.GetConsumingEnumerable())
                {

                    var payload = new RawEvent(response.raw_request.id
                                            , Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response.core_response.events))
                                            , null
                                            , "PatternPeriodsChanged");

                    RawEvent input_raw_event;

                    if (raw_event_requests.TryRemove(response.raw_request.id, out input_raw_event))
                    {
                        event_store.Publish(payload, input_raw_event.ToPointer());
                    }
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
                                            JToken.Parse(Encoding.UTF8.GetString(ev.data)),
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
