using System;
using System.Collections.Concurrent;
using LbspSOA;
using Registration.Domain;

namespace Registration.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            var request_queue = new BlockingCollection<RawRequest<RegistrationWorld>>();

            var response_queue = new BlockingCollection<RawResponse<RegistrationWorld>>();

            var service = new LbspService<RegistrationWorld>(request_queue, response_queue, RegistrationWorld.seed_world());

            var request_handler = 
                new RequestHandler<RegistrationWorld>(request_queue, 
                                                    response_queue, 
                                                    new GESEventStore(Registration.Interface.NameService.ContextName), 
                                                    new Router());

            service.replay(request_handler.get_history());

            service.start();

            request_handler.start_listening(Gateway.Interface.NameService.ContextName);

            Console.ReadLine();

            request_handler.stop_listening();

            service.stop();

        }
    }
}
