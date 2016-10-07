namespace LbspSOA
{
    public interface IRouter
    {
        bool is_route_handler_defined(string route_name);
        object get_handler(string route_name);
    }
}