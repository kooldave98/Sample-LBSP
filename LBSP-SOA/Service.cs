using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeStructures;
using Newtonsoft.Json;

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
            Response<W> response;

            try
            {
                var trigger = request.trigger_as_json.To(request.handler.trigger_type());

                var pattern_request = new Request<W, ITrigger>(world, trigger as ITrigger);                

                response = request.handler.handle(pattern_request);                
            }
            catch (JsonSerializationException e)
            {
                var trigger_error_type = typeof(TriggerInitialisationError<>);

                var generic_trigger_error_type = trigger_error_type.MakeGenericType(request.handler.trigger_type());

                var trigger_error = Activator.CreateInstance(generic_trigger_error_type);

                response = new Response<W>(world, (trigger_error as ITrigger).ToEnumerable());
            }
            catch (Exception e)
            {
                response = new Response<W>(world, new UnknownError().ToEnumerable());
            }
            

            world = response.world;//This is very important

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