﻿using LbspSOA;
using Registration.Domain;

namespace Registration.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            var context_name = Registration.Interface.NameService.ContextName;
            var seed_world = RegistrationWorld.seed_world();
            var router = new Router();

            new ServiceBootstrap<RegistrationWorld>(context_name, seed_world, router)
                .replay()
                .listen_to(Gateway.Interface.NameService.ContextName)
                .StartService();
        }
    }
}
