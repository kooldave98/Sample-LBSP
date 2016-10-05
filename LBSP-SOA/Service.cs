using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeStructures;
using System.Linq;

namespace LbspSOA
{
    public class LbspService<W> where W : IWorld
    {
        private W _world;

        private RequestHandler<W> _requestHandler;

        private IRouter<W> _router;

        private GESEventStore _eventStore;

        private BlockingCollection<RawRequest<W>> _requestQueue;

        private BlockingCollection<RawResponse<W>> _responseQueue;

        public BlockingCollection<RawRequest<W>> RequestQueue => _requestQueue;
        public BlockingCollection<RawResponse<W>> ResponseQueue => _responseQueue;

        public HashSet<string> Streams { get; set; }

        public bool ReplayHistory { get; set; }
                

        public LbspService(W initialWorld, string context, IRouter<W> router)
        {
            _world = initialWorld;

            _eventStore = new GESEventStore(context);

            _router = router;

            _requestQueue = new BlockingCollection<RawRequest<W>>();

            _responseQueue = new BlockingCollection<RawResponse<W>>();

            _requestHandler = new RequestHandler<W>(_requestQueue, _responseQueue, _eventStore, _router);

            Streams = new HashSet<string>();
        }


        public void Start()
        {
            if (ReplayHistory)
            {
                replay(_requestHandler.get_history());
            }

            _requestHandler.start_listening(Streams.ToArray());

            Task.Run(() =>
            {
                foreach (var request in _requestQueue.GetConsumingEnumerable())
                {
                    var response = Process(request);

                    _responseQueue.Add(response);
                }
            });

            Console.WriteLine("Started domain service...");
        }

        private RawResponse<W> Process(RawRequest<W> request)
        {
            var patternRequest = new Request<W>(_world, request.memento);

            Response<W> response;

            try
            {
                response = request.handler(patternRequest);
                //This is very important
                _world = response.world;
            }
            catch
            {
                response = new Response<W>(_world, new UnknownError().ToEnumerable());
            }

            return new RawResponse<W>(request, response);
        }

        public void replay(IEnumerable<RawRequest<W>> requests)
        {
            Console.WriteLine("Started re-playing domain events to build world...");
            foreach (var request in requests)
            {
                Process(request);
                Console.WriteLine($"replayed request: {request.id}");
            }

            Console.WriteLine("Finished re-playing domain events ...");
        }

        public void Stop()
        {
            _requestHandler.stop_listening();
            //Todo: 
            //What do we do here ?
            //Do we really even need this ?
        }

        //readonly BlockingCollection<RawRequest<W>> request_queue;
        //readonly BlockingCollection<RawResponse<W>> response_queue;
    }
}
