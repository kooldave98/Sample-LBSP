using System.Collections.Generic;

namespace LbspSOA
{
    public interface IBehavior<W> where W : IWorld
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

        public Response(W world, IEnumerable<ITrigger> events)
        {
            this.world = world;
            this.events = events;
        }
    }

    public interface IWorld { }
}
