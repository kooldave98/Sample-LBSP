using System;

namespace LbspSOA
{
    public interface IRouter<W> where W : IWorld
    {
        bool is_route_handler_defined(string route_name);
        Func<Request<W>, Response<W>> get_handler(string route_name);
    }
}