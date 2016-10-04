using System;
using System.Threading.Tasks;
using System.Web.Http;
using CodeStructures;
using LbspSOA;
using Query.Interface;
using Registration.Interface;

namespace Gateway.Controllers
{
    public class CreateHostController : ApiController
    {
        [Route("api/create-parking-host")]
        public async Task<bool> Get(string username, string email)
        {
            var trigger = new CreateParkingHost(Guid.NewGuid(), username, email);

            var event_store = new GESEventStore(Gateway.Interface.NameService.ContextName);

            var outgoing_event = new RawEvent(Guid.NewGuid(),
                                                trigger.ToBytes(),
                                                null,
                                                nameof(CreateParkingHost)
                                                );

            var tcs = new TaskCompletionSource<bool>();

            event_store.SubscribeHenceForth(Query.Interface.NameService.ContextName, recorded_event => {
                if(recorded_event.raw_event.type == nameof(ParkingHostMaterialised)
                    &&
                    recorded_event.raw_event.data.ToJsonDynamic().host_id == trigger.host_id)
                {
                    tcs.TrySetResult(true);
                }
            });

            event_store.Publish(outgoing_event.ToEnumerable());

            return await tcs.Task;
        }
    }
}
