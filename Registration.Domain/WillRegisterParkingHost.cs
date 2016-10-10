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
            var errors = validate(request);
            
            if (errors.Any())
            {
                return new Response<RegistrationWorld>(request.world, errors);
            }            

            var new_host = new ParkingHost(request.trigger.host_id, request.trigger.username, request.trigger.email);

            var new_world = new RegistrationWorld(request.world.hosts.Union(new_host.ToEnumerable()), request.world.guests);

            return new Response<RegistrationWorld>(new_world, new ParkingHostRegistered(new_host.host_id, new_host.username));
        }

        private IEnumerable<AnErrorTrigger> validate(Request<RegistrationWorld, RegisterParkingHost> request)
        {
            var errors = new HashSet<AnErrorTrigger>();

            //validation 1: prevent duplicate id
            if (request.world.hosts.Any(h => h.host_id == request.trigger.host_id))
            {
                errors.Add(new ParkingHostIDTaken(request.trigger.host_id));
            }

            //validation 2: prevent duplicate names
            if (request.world.hosts.Any(h => h.username == request.trigger.username))
            {
                errors.Add(new ParkingHostUsernameTaken(request.trigger.username));
            }

            //validation 2: prevent duplicate names
            if (request.world.hosts.Any(h => h.email == request.trigger.email))
            {
                errors.Add(new ParkingHostEmailTaken(request.trigger.email));
            }

            return errors;
        }
    }
}
