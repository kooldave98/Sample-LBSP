using System.Collections.Generic;
using LbspSOA;
using Registration.Domain;
using Registration.Interface;

namespace Registration.Service
{
    public class Router : IRouter<RegistrationWorld>
    {
        public bool is_route_handler_defined(string route_name)
        {
            return route_dictionary.ContainsKey(route_name);
        }

        ITriggerHandler<RegistrationWorld, ITrigger> IRouter<RegistrationWorld>.get_handler(string route_name)
        {
            return route_dictionary[route_name];
        }

        private readonly Dictionary<string, ITriggerHandler<RegistrationWorld, ITrigger>> route_dictionary;

        public Router()
        {
            route_dictionary = new Dictionary<string, ITriggerHandler<RegistrationWorld, ITrigger>>()
            {
                { nameof(RegisterParkingHost), new WillCreateParkingHost() as ITriggerHandler<RegistrationWorld, ITrigger> },
                { nameof(RegisterParkingHost), new WillCreateParkingGuest() as ITriggerHandler<RegistrationWorld, ITrigger> },
            };
        }
    }
}
