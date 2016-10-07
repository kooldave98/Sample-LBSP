using System;
using System.Threading.Tasks;
using System.Web.Http;
using LbspSOA;
using Query.Interface;
using Registration.Interface;
using System.Reactive.Linq;

namespace Gateway.Controllers
{
    public class RegisterParkingHostController : ApiController
    {
        [Route("api/register-parking-host")]
        public async Task<object> Post(string username, string email)
        {
            var trigger = new RegisterParkingHost(Guid.NewGuid(), username, email);

            var event_store = new GESEventStore(Gateway.Interface.NameService.ContextName);            

            var tcs = new TaskCompletionSource<object>();

            event_store
                .NewEvents(Query.Interface.NameService.ContextName)
                .Where(re => re.raw_event.type == nameof(ParkingHostMaterialised))
                .Where(re => re.raw_event.data.To<ParkingHostMaterialised>().host_id == trigger.host_id)
                .Subscribe(re => tcs.SetResult(null));

            event_store.Publish(trigger.ToRawEvent());

            return await tcs.Task;
        }
    }
}
