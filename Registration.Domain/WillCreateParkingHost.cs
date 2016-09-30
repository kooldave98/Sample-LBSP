using System.Collections.Generic;
using System.Linq;
using CodeStructures;
using LbspSOA;
using Registration.Interface;

namespace Registration.Domain
{
    public class WillCreateParkingHost : IBehavior<RegistrationWorld>
    {
        public Response<RegistrationWorld> handle(Request<RegistrationWorld> request)
        {
            var errors = new HashSet<ITrigger>();

            if (request.world.hosts.Any(h => h.host_id == request.trigger.host_id))
            {
                errors.Add(new ParkingGuestIDTaken(request.trigger.host_id));
            }

            if (request.world.hosts.Any(h => h.username == request.trigger.username))
            {
                errors.Add(new ParkingGuestUsernameTaken(request.trigger.username));
            }

            if (errors.Any())
            {
                return new Response<RegistrationWorld>(request.world, errors);
            }

            var new_host = new ParkingHost(request.trigger.host_id, request.trigger.username);

            var new_world = new RegistrationWorld(request.world.hosts.Union(new_host.ToEnumerable()), request.world.guests);

            return new Response<RegistrationWorld>(new_world, new ParkingHostCreated(new_host.host_id, new_host.username).ToEnumerable());
        }
    }
}
