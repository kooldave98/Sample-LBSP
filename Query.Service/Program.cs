using System;
using LbspSOA;
using Query.Domain;

namespace Query.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new LbspService<QueryWorld>(QueryWorld.seed_world(), Query.Interface.NameService.ContextName, new Router());

            //No replay in Query service

            service.Streams.Add(Registration.Interface.NameService.ContextName);

            service.Start();

            Console.ReadLine();

            service.Stop();
        }
    }
}