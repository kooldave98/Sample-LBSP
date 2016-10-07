namespace LbspSOA
{
    public interface IRouter<W> where W : IWorld
    {
        bool is_route_handler_defined(string route_name);
        ITriggerHandler<W, ITrigger> get_handler(string route_name);
    }
}