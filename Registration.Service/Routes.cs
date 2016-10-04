using System;
using System.Collections.Generic;
using LbspSOA;
using Registration.Domain;
using Registration.Interface;

namespace Registration.Service
{
    public class Router : IRouter<RegistrationWorld>
    {
        public Func<Request<RegistrationWorld>, Response<RegistrationWorld>> get_handler(string route_name)
        {
            return route_dictionary[route_name];
        }

        public bool is_route_handler_defined(string route_name)
        {
            return route_dictionary.ContainsKey(route_name);
        }

        private readonly Dictionary<string, Func<Request<RegistrationWorld>, Response<RegistrationWorld>>> route_dictionary;

        public Router()
        {
            route_dictionary = new Dictionary<string, Func<Request<RegistrationWorld>, Response<RegistrationWorld>>>()
            {
                { nameof(RegisterParkingHost), new WillCreateParkingHost().handle },

                { nameof(RegisterParkingGuest), new WillCreateParkingGuest().handle },
            };
        }
    }
}
