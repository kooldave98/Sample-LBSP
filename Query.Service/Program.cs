using System;
using System.Collections.Concurrent;
using CodeStructures;
using LbspSOA;
using Query.Domain;
using Registration.Interface;

namespace Query.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            var request_queue = new BlockingCollection<RawRequest<QueryWorld>>();

            var response_queue = new BlockingCollection<RawResponse<QueryWorld>>();

            var service = new LbspService<QueryWorld>(request_queue, response_queue, QueryWorld.seed_world());

            var request_handler =
                new RequestHandler<QueryWorld>(request_queue,
                                                    response_queue,
                                                    new GESEventStore(Query.Interface.NameService.ContextName),
                                                    new Router());

            //No replay in Query service

            service.start();

            request_handler.start_listening(Registration.Interface.NameService.ContextName);

            Console.ReadLine();

            request_handler.stop_listening();

            service.stop();
        }
    }
}