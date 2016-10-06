using LbspSOA;
using Query.Domain;

namespace Query.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            var context_name = Query.Interface.NameService.ContextName;
            var seed_world = QueryWorld.seed_world();
            var router = new Router();

            new 
                ServiceBootstrap<QueryWorld>(context_name, seed_world, router)                
                .listen_to(Registration.Interface.NameService.ContextName)
                .StartService();
        }
    }
}