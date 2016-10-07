using System;
using System.Threading.Tasks;
using System.Web.Http;
using CodeStructures;
using LbspSOA;
using Query.Interface;
using Registration.Interface;
using System.Reactive.Linq;

namespace Gateway.Controllers
{
    public class RegisterParkingHostController : ApiController
    {
        [Route("api/register-parking-host")]
        public async Task<object> Get(RegisterParkingHost trigger)
        {
            var event_store = new GESEventStore(Gateway.Interface.NameService.ContextName);

            var outgoing_event = trigger.ToRawEvent();

            var tcs = new TaskCompletionSource<object>();

            event_store
                .NewEvents(Query.Interface.NameService.ContextName)
                .Where(re => re.raw_event.type == nameof(ParkingHostMaterialised))
                .Where(re => re.raw_event.data.To<ParkingHostMaterialised>().host_id == trigger.host_id)
                .Subscribe(re => tcs.SetResult(null));

            event_store.Publish(outgoing_event.ToEnumerable());

            return await tcs.Task;
        }
    }
}
