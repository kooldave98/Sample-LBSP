using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LbspSOA
{
    public class RequestHandler<W> where W : IWorld
    {
        private void handle(RecordedRawEvent recorded_event)
        {
            Console.WriteLine($"Received event with id: {recorded_event.raw_event.id}");

            if (!router.is_route_handler_defined(recorded_event.raw_event.type))
            {
                Console.WriteLine($"No handler defined for event with id: {recorded_event.raw_event.id}");

                return;
            }

            var request = new RawRequest<W>(recorded_event.raw_event.id,
                                            recorded_event.raw_event.data.ToJsonString(),
                                            recorded_event.raw_event.type,
                                            router.get_handler(recorded_event.raw_event.type));

            var responder_added = responders.TryAdd(recorded_event.raw_event.id, response => {
                response
                    .core_response
                    .match(
                        is_success: events =>
                        {

                            var payload =
                                events.Select(ev => ev.ToRawEvent(response.raw_request.id));

                            //Potential data inconsistencies between removing above, and publishing below
                            event_store.CommitAndPublish(recorded_event, payload);
                        },
                        is_error: errors =>
                        {

                            var payload =
                                errors.Select(ev => ev.ToRawEvent(response.raw_request.id));

                            //Potential data inconsistencies between removing above, and publishing below
                            event_store.PublishErrors(recorded_event, payload);
                        }
                    );


                Console.WriteLine($"Finished event with id: {response.raw_request.id}");
            });

            if(!responder_added)
            {
                throw new Exception("Responder could not be added");
            }

            requests.Add(request);
        }

        public void start_listening(IEnumerable<string> streams)
        {
            start_listening(streams.ToArray());
        }

        public void start_listening(params string[] streams)
        {
            Task.Run(() =>
            {
                foreach (var response in responses.GetConsumingEnumerable())
                {
                    Action<RawResponse<W>> responder;
                    var responder_found = responders.TryRemove(response.raw_request.id, out responder);

                    if(!responder_found)
                    {
                        throw new Exception("Could not find responder");
                    }

                    responder(response);
                }
            });

            
            event_store
                .AllUnprocessedEvents(streams)
                .Subscribe(handle);
            

            Console.WriteLine($"Started listening to event-store for events...");
        }

        public IEnumerable<RawRequest<W>> get_history()
        {
            return
                event_store
                .get_history()
                .Select(ev =>
                            new RawRequest<W>(ev.id,
                                            ev.data.ToJsonString(),
                                            ev.type,
                                            router.get_handler(ev.type)));
        }

        public void stop_listening()
        {
            event_store.unsubscribe_all();
        }

        public RequestHandler(BlockingCollection<RawRequest<W>> the_requests
                            , BlockingCollection<RawResponse<W>> the_responses
                            , EventStore the_event_store)
        {
            requests = the_requests;
            responses = the_responses;
            event_store = the_event_store;

            router = new Router();
        }

        private ConcurrentDictionary<Guid, Action<RawResponse<W>>> responders = new ConcurrentDictionary<Guid, Action<RawResponse<W>>>();

        private readonly BlockingCollection<RawRequest<W>> requests;
        private readonly BlockingCollection<RawResponse<W>> responses;
        private readonly Router router;
        private readonly EventStore event_store;
    }
}
