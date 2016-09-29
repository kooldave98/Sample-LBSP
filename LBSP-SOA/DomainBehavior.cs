using System.Collections.Generic;

namespace LbspSOA
{
    public interface IBehavior<T, W> where T : ITrigger where W : IWorld
    {
        Response<W> handle(Request<T, W> request);
    }

    public sealed class Request<T, W> where T : ITrigger where W : IWorld
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

        public Response(W world, IEnumerable<ITrigger> events)
        {
            this.world = world;
            this.events = events;
        }
    }

    public interface IWorld { }
}
