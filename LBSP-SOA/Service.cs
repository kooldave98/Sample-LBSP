using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeStructures;

namespace LbspSOA
{
    public class LbspService<W> where W : IWorld
    {
        public LbspService
                (BlockingCollection<RawRequest<W>> request_queue
                , BlockingCollection<RawResponse<W>> response_queue
                , W initial_world)
        {

            this.request_queue = request_queue;
            this.response_queue = response_queue;
            this.world = initial_world;
        }


        public void start()
        {
            Task.Run(() =>
            {
                foreach (var request in request_queue.GetConsumingEnumerable())
                {
                    var response = process(request);

                    response_queue.Add(response);
                }

            });

            Console.WriteLine("Started domain service...");
        }

        private RawResponse<W> process(RawRequest<W> request)
        {
            var pattern_request = new Request<W>(world, request.memento);

            Response<W> response;

            try
            {
                response = request.handler(pattern_request);
                //This is very important
                world = response.world;
            }
            catch(Exception e)
            {
                response = new Response<W>(world, new UnknownError().ToEnumerable());
            }

            return new RawResponse<W>(request, response);
        }

        public void replay(IEnumerable<RawRequest<W>> requests)
        {
            Console.WriteLine("Started re-playing domain events to build world...");
            foreach (var request in requests)
            {
                process(request);
                Console.WriteLine($"replayed request: {request.id}");
            }

            Console.WriteLine("Finished re-playing domain events ...");
        }

        public void stop()
        {
            //Todo: 
            //What do we do here ?
            //Do we really even need this ?
        }

        private W world;

        readonly BlockingCollection<RawRequest<W>> request_queue;
        readonly BlockingCollection<RawResponse<W>> response_queue;
    }
}
