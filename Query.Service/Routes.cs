using System;
using System.Collections.Generic;
using LbspSOA;
using Query.Domain;
using Registration.Interface;

namespace Query.Service
{
    public class Router : IRouter<QueryWorld>
    {
        public Func<Request<QueryWorld>, Response<QueryWorld>> get_handler(string route_name)
        {
            return route_dictionary[route_name];
        }

        public bool is_route_handler_defined(string route_name)
        {
            return route_dictionary.ContainsKey(route_name);
        }

        private readonly Dictionary<string, Func<Request<QueryWorld>, Response<QueryWorld>>> route_dictionary;

        public Router()
        {
            route_dictionary = new Dictionary<string, Func<Request<QueryWorld>, Response<QueryWorld>>>()
            {
                { nameof(ParkingHostRegistered), new WillMaterialiseParkingHost().handle },
            };
        }
    }
}
