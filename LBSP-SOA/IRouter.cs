using System;

namespace LbspSOA
{
    public interface IRouter<W> where W : IWorld
    {
        Func<Request<W>, Response<W>> get_handler(string route_name);
    }
}