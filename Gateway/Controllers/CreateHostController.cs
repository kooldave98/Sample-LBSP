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
            var trigger = new RegisterParkingHost(Guid.NewGuid(), username, email);

            var event_store = new GESEventStore(Gateway.Interface.NameService.ContextName);

            var outgoing_event = new RawEvent(Guid.NewGuid(),
                                                trigger.ToBytes(),
                                                null,
                                                nameof(RegisterParkingHost)
                                                );

            var tcs = new TaskCompletionSource<bool>();

            Func<RecordedRawEvent, bool> predicate = 
                re => re.raw_event.type == nameof(ParkingHostMaterialised)
                        && re.raw_event.data.ToJsonDynamic().host_id == trigger.host_id;

            event_store
                .SubscribeHenceForth(
                    Query.Interface.NameService.ContextName, 
                    re => tcs.TrySetResult(true), 
                    predicate
                );

            event_store.Publish(outgoing_event.ToEnumerable());

            return await tcs.Task;
        }
    }
}
