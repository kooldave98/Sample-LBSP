using System;
using System.Collections.Generic;
using System.Linq;

namespace LbspSOA
{
    public interface ITriggerHandler<W> where W : IWorld
    {
        Response<W> handle(Request<W> request);
    }

    public sealed class Request<W> where W : IWorld
    {
        public W world;
        public dynamic trigger;

        public Request(W world, dynamic trigger)
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

        public Response(W world, IEnumerable<ITrigger> events)
        {
            this.world = world;
            this.events = events;
        }
    }

    public interface IWorld { }
}
