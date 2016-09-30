using System;
using System.Collections.Generic;
using System.Linq;
using CodeStructures;
using LbspSOA;
using Registration.Interface;

namespace Registration.Domain
{
    public class WillCreateParkingGuest : IBehavior<RegistrationWorld>
    {
        public Response<RegistrationWorld> handle(Request<RegistrationWorld> request)
        {
            var trigger = new CreateParkingGuest((Guid)request.trigger.guest_id, (string)request.trigger.username);

            var errors = new HashSet<ITrigger>();

            if(request.world.guests.Any(g=>g.guest_id == trigger.guest_id))
            {
                errors.Add(new ParkingGuestIDTaken(trigger.guest_id));
            }

            if (request.world.guests.Any(g => g.username == trigger.username))
            {
                errors.Add(new ParkingGuestUsernameTaken(trigger.username));
            }

            if (errors.Any())
            {
                return new Response<RegistrationWorld>(request.world, errors);
            }

            var new_guest = new ParkingGuest(trigger.guest_id, trigger.username);

            var new_world = new RegistrationWorld(request.world.hosts, request.world.guests.Union(new_guest.ToEnumerable()));

            return new Response<RegistrationWorld>(new_world, new ParkingGuestCreated(new_guest.guest_id, new_guest.username).ToEnumerable());
        }
    }
}
