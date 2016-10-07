using System;
using LbspSOA;
using Query.Interface;
using Registration.Interface;

namespace Query.Domain
{
    public class WillMaterialiseParkingHost : ITriggerHandler<QueryWorld, ParkingHostRegistered>
    {
        public Response<QueryWorld> handle(Request<QueryWorld, ParkingHostRegistered> request)
        {
            var repository = new Repository<Host>(request.world.db_context);

            repository.add(new Host()
            {
                HostID = request.trigger.host_id,
                Username = request.trigger.username
            });

            request.world.db_context.SaveChanges();

            return
                new Response<QueryWorld>(request.world, 
                                        new ParkingHostMaterialised(request.trigger.host_id));

        }

        public Type trigger_type()
        {
            return typeof(ParkingHostRegistered);
        }
    }
}
