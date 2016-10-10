using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace LbspSOA
{
    public class ServiceBootstrap<W> where W : IWorld
    {
        BlockingCollection<RawRequest<W>> request_queue;
        BlockingCollection<RawResponse<W>> response_queue;

        LbspService<W> service;

        RequestHandler<W> request_handler;

        public ServiceBootstrap(string context_name, W init_world)
        {
            request_queue = new BlockingCollection<RawRequest<W>>();
            response_queue = new BlockingCollection<RawResponse<W>>();
            service = new LbspService<W>(request_queue, response_queue, init_world);

            request_handler =
                new RequestHandler<W>(request_queue,
                                                    response_queue,
                                                    new EventStore(context_name));
        }

        public ServiceBootstrap<W> replay()
        {
            service.replay(request_handler.get_history());
            return this;
        }

        public ServiceBootstrap<W> listen_to(string stream)
        {
            streamsToListen.Add(stream);

            return this;
        }

        private HashSet<string> streamsToListen = new HashSet<string>();

        public void StartService()
        {
            service.start();

            request_handler.start_listening(streamsToListen);

            Console.WriteLine("Press any key to stop service...");
            Console.ReadLine();

            Console.WriteLine("Warning the service will be stopped, are you sure ?");
            Console.ReadLine();

            request_handler.stop_listening();

            service.stop();
        }
    }
}
