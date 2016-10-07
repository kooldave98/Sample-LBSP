using System.Collections.Generic;
using LbspSOA;
using Registration.Domain;
using Registration.Interface;

namespace Registration.Service
{
    public class Router : IRouter
    {
        public bool is_route_handler_defined(string route_name)
        {
            return route_dictionary.ContainsKey(route_name);
        }

        public object get_handler(string route_name)
        {
            return route_dictionary[route_name];
        }

        private readonly Dictionary<string, object> route_dictionary;

        public Router()
        {
            route_dictionary = new Dictionary<string, object>()
            {
                { nameof(RegisterParkingHost), new WillRegisterParkingHost() },
                { nameof(RegisterParkingGuest), new WillRegisterParkingGuest() },
            };
        }
    }
}
