using System.Collections.Generic;
using System.Linq;
using CodeStructures;
using LbspSOA;
using Registration.Interface;

namespace Registration.Domain
{
    public class WillRegisterParkingHost : ITriggerHandler<RegistrationWorld, RegisterParkingHost>
    {
        public Response<RegistrationWorld> handle(Request<RegistrationWorld, RegisterParkingHost> request)
        {
            var world = request.world;

            var errors = new HashSet<ITrigger>();

            //validation 1: prevent duplicate id
            if (world.hosts.Any(h => h.host_id == request.trigger.host_id))
            {
                errors.Add(new ParkingHostIDTaken(request.trigger.host_id));
            }

            //validation 2: prevent duplicate names
            if (world.hosts.Any(h => h.username == request.trigger.username))
            {
                errors.Add(new ParkingHostUsernameTaken(request.trigger.username));
            }

            //validation 2: prevent duplicate names
            if (world.hosts.Any(h => h.email == request.trigger.email))
            {
                errors.Add(new ParkingHostEmailTaken(request.trigger.email));
            }

            if (errors.Any())
            {
                return new Response<RegistrationWorld>(world, errors);
            }
            //after this point everything is good to go

            var new_host = new ParkingHost(request.trigger.host_id, request.trigger.username, request.trigger.email);

            var new_world = new RegistrationWorld(world.hosts.Union(new_host.ToEnumerable()), world.guests);

            return new Response<RegistrationWorld>(new_world, new ParkingHostRegistered(new_host.host_id, new_host.username));
        }
    }
}
