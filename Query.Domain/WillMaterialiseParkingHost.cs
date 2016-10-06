using CodeStructures;
using LbspSOA;
using Query.Interface;
using Registration.Interface;

namespace Query.Domain
{
    public class WillMaterialiseParkingHost : ITriggerHandler<QueryWorld>
    {
        public Response<QueryWorld> handle(Request<QueryWorld> request)
        {
            return
            (ParkingHostRegistered.from_dynamic(request.trigger) as IMaybe<ParkingHostRegistered>)
                .Match(trigger =>
                {
                    var repository = new Repository<Host>(request.world.db_context);

                    repository.add(new Host()
                    {
                        HostID = trigger.host_id,
                        Username = trigger.username
                    });

                    request.world.db_context.SaveChanges();

                    return
                        new Response<QueryWorld>(request.world, new ParkingHostMaterialised(trigger.host_id).ToEnumerable());

                },() => type_init_error(request));
        }

        private static Response<QueryWorld> type_init_error(Request<QueryWorld> request)
        {
            return new Response<QueryWorld>(request.world, new TriggerInitialisationError<ParkingHostRegistered>().ToEnumerable());
        }
    }
}
