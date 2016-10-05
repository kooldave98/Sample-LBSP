using System;
using LbspSOA;
using Registration.Domain;

namespace Registration.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new LbspService<RegistrationWorld>(RegistrationWorld.seed_world(), Registration.Interface.NameService.ContextName, new Router());

            service.ReplayHistory = true;

            service.Streams.Add(Gateway.Interface.NameService.ContextName);

            service.Start();

            Console.ReadLine();

            service.Stop();

        }
    }
}
