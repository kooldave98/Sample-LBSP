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
    public class CreateHostController : ApiController
    {
        [Route("api/create-parking-host")]
        public async Task<object> Get(string username, string email)
        {
            var event_store = new GESEventStore(Gateway.Interface.NameService.ContextName);

            var trigger = new RegisterParkingHost(Guid.NewGuid(), username, email);

            var outgoing_event = trigger.ToRawEvent();

            var tcs = new TaskCompletionSource<object>();

            event_store
                .NewEvents(Query.Interface.NameService.ContextName)
                .Where(re => re.raw_event.type == nameof(ParkingHostMaterialised))
                .Where(re => re.raw_event.data.ToJsonDynamic().host_id == trigger.host_id)
                .Subscribe(re => tcs.SetResult(null));

            event_store.Publish(outgoing_event.ToEnumerable());

            return await tcs.Task;
        }
    }
}
