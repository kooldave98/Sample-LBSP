using System;
using System.Collections.Generic;
using System.Linq;

namespace LbspSOA
{
    public sealed class Router
    {
        public bool is_route_handler_defined(string route_name)
        {
            return route_dictionary.ContainsKey(route_name);
        }

        public HandlerTypeInfo get_handler(string route_name)
        {
            return route_dictionary[route_name];
        }

        private readonly Dictionary<string, HandlerTypeInfo> route_dictionary;

        public Router()
        {
            route_dictionary =
                AppDomain
                .CurrentDomain
                .GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(type => typeof(ITriggerHandler<,>).IsAssignableFrom(type))
                .Select(type => new HandlerTypeInfo(type))
                .ToDictionary(i => i.trigger_type.AssemblyQualifiedName);
        }
    }

    public class HandlerTypeInfo
    {
        public readonly Type handler_type;
        public readonly Type trigger_type;
        public readonly Type world_type;
        public readonly Type request_type;

        public readonly Type trigger_initialisation_error_type;

        public HandlerTypeInfo(Type handler_type)
        {
            this.handler_type = handler_type;

            trigger_type = handler_type.GetGenericArguments().Single(a => typeof(ITrigger).IsAssignableFrom(a));
            world_type = handler_type.GetGenericArguments().Single(a => typeof(IWorld).IsAssignableFrom(a));

            request_type = typeof(Request<,>).MakeGenericType(world_type, trigger_type);

            trigger_initialisation_error_type = typeof(TriggerInitialisationError<>).MakeGenericType(trigger_type);
        }
    }
}