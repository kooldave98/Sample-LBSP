using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using LbspSOA;
using Registration.Domain;
using Registration.Interface;

namespace Registration.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            var request_queue = new BlockingCollection<RawRequest<RegistrationWorld>>();

            var response_queue = new BlockingCollection<RawResponse<RegistrationWorld>>();

            var service = new LbspService<RegistrationWorld>(request_queue, response_queue, RegistrationWorld.seed_world());

            var request_handler = new RequestHandler<RegistrationWorld>(request_queue, response_queue, new GESEventStore(nameof(RegistrationWorld)), new Router());

            service.replay(request_handler.get_history());

            service.start();

            request_handler.start_listening();

            Console.ReadLine();

            request_handler.stop_listening();

            service.stop();

        }
    }

    public class Router : IRouter<RegistrationWorld>
    {
        public Func<Request<RegistrationWorld>, Response<RegistrationWorld>> get_handler(string route_name)
        {
            return route_dictionary[route_name];
        }

        private readonly Dictionary<string, Func<Request<RegistrationWorld>, Response<RegistrationWorld>>> route_dictionary;

        public Router()
        {
            route_dictionary = new Dictionary<string, Func<Request<RegistrationWorld>, Response<RegistrationWorld>>>()
            {
                { nameof(CreateParkingHost), new WillCreateParkingHost().handle },

                { nameof(CreateParkingGuest), new WillCreateParkingGuest().handle },
            };
        }
    }
}
