using System;
using System.Collections.Generic;
using System.Linq;

namespace LbspSOA
{
    public interface ITriggerHandler<W, T> where W : IWorld where T : ITrigger
    {
        Response<W> handle(Request<W, T> request);
        Type trigger_type();
    }

    public sealed class Request<W, T> where W : IWorld where T : ITrigger
    {
        public W world;
        public T trigger;

        public Request(W world, T trigger)
        {
            this.world = world;
            this.trigger = trigger;
        }
    }

    public sealed class Response<W> where W : IWorld
    {
        public W world;
        public IEnumerable<ITrigger> events;

        public void match(Action<IEnumerable<ITrigger>> is_success, Action<IEnumerable<IErrorTrigger>> is_error)
        {
            if(events.has_errors())
            {
                is_error(events.OfType<IErrorTrigger>());

                return;
            }

            is_success(events);
        }

        public Response(W world, params ITrigger[] events)
        {
            this.world = world;
            this.events = events;
        }

        public Response(W world, IEnumerable<ITrigger> events) 
            : this(world, events.ToArray())
        {
        }
    }

    public interface IWorld { }
}
