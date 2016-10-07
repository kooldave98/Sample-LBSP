using System;
using System.Collections.Generic;
using LbspSOA;
using Query.Domain;
using Registration.Interface;

namespace Query.Service
{
    public class Router : IRouter<QueryWorld>
    {
        public ITriggerHandler<QueryWorld, ITrigger> get_handler(string route_name)
        {
            return route_dictionary[route_name];
        }

        public bool is_route_handler_defined(string route_name)
        {
            return route_dictionary.ContainsKey(route_name);
        }

        private readonly Dictionary<string, ITriggerHandler<QueryWorld, ITrigger>> route_dictionary;

        public Router()
        {
            route_dictionary = new Dictionary<string, ITriggerHandler<QueryWorld, ITrigger>>()
            {
                { nameof(ParkingHostRegistered), new WillMaterialiseParkingHost() as ITriggerHandler<QueryWorld, ITrigger> },
            };
        }
    }
}
