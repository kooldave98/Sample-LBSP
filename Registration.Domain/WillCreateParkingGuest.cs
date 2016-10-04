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
            return
            (RegisterParkingGuest
                .from_dynamic(request.trigger) as IMaybe<RegisterParkingGuest>)
                .Match(
                trigger =>
                {
                    var errors = new HashSet<ITrigger>();

                    if (request.world.guests.Any(g => g.guest_id == trigger.guest_id))
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

                    return new Response<RegistrationWorld>(new_world, new ParkingGuestRegistered(new_guest.guest_id, new_guest.username).ToEnumerable());
                },
                () => type_init_error(request));

            
        }

        private static Response<RegistrationWorld> type_init_error(Request<RegistrationWorld> request)
        {
            return new Response<RegistrationWorld>(request.world, new TriggerInitialisationError<RegisterParkingGuest>().ToEnumerable());
        }

    }
}
