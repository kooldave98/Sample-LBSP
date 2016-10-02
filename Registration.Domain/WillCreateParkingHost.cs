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
            return
            (CreateParkingHost
                .from_dynamic(request.trigger) as IMaybe<CreateParkingHost>)
                .Match(
                    trigger => {
                        //has value

                        var world = request.world;

                        var errors = new HashSet<ITrigger>();

                        //validation 1: prevent duplicate id
                        if (world.hosts.Any(h => h.host_id == trigger.host_id))
                        {
                            errors.Add(new ParkingGuestIDTaken(trigger.host_id));
                        }

                        //validation 2: prevent duplicate names
                        if (world.hosts.Any(h => h.username == trigger.username))
                        {
                            errors.Add(new ParkingGuestUsernameTaken(trigger.username));
                        }

                        //validation 2: prevent duplicate names
                        if (world.hosts.Any(h => h.email == trigger.email))
                        {
                            errors.Add(new ParkingGuestUsernameTaken(trigger.username));
                        }

                        if (errors.Any())
                        {
                            return new Response<RegistrationWorld>(world, errors);
                        }
                        //after this point everything is good to go

                        var new_host = new ParkingHost(trigger.host_id, trigger.username, trigger.email);

                        var new_world = new RegistrationWorld(world.hosts.Union(new_host.ToEnumerable()), world.guests);

                        return new Response<RegistrationWorld>(new_world, new ParkingHostCreated(new_host.host_id, new_host.username).ToEnumerable());




                    },
                    () => type_init_error(request)
                );
        }

        private static Response<RegistrationWorld> type_init_error(Request<RegistrationWorld> request)
        {
            return new Response<RegistrationWorld>(request.world, new TriggerInitialisationError<CreateParkingGuest>().ToEnumerable());
        }
    }
}
