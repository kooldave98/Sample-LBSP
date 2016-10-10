using System;
using System.Threading.Tasks;
using System.Web.Http;
using LbspSOA;
using Query.Interface;
using Registration.Interface;
using System.Reactive.Linq;

namespace Gateway.Controllers
{
    /*
     *                                          ==Todo==
     * 1. SynchronousCommandBuilder: To hide in an abstraction the internals of publishing and waiting, and timeout
     * 2. remember to conserve resources /avoid memory leaks => event_store.unsubscribe_all();
     * 3. Need to handle case when ===> No handler defined for event with id
     */
    public class RegisterParkingHostController : ApiController
    {        
        private EventStore event_store = new EventStore(Gateway.Interface.NameService.ContextName);

        [Route("api/register-parking-host")]
        public async Task<string> Post(RegisterParkingHost trigger)
        {
            var response = await send(trigger);            

            if (response)
            {
                return "success";
            }

            return "an error occurred";
        }

        private async Task<bool> send(RegisterParkingHost trigger)
        {
            var trigger_as_raw_event = trigger.ToRawEvent();

            var tcs = new TaskCompletionSource<bool>();

            event_store
                .NewEvents(Query.Interface.NameService.ContextName, $"{Gateway.Interface.NameService.ContextName}-Errors")
                .Subscribe(re => {

                    if (re.raw_event.type == nameof(ParkingHostMaterialised) &&
                        re.raw_event.data.To<ParkingHostMaterialised>().host_id == trigger.host_id)
                    {
                        tcs.TrySetResult(true);
                    }

                    if (re.origin_stream == $"{Gateway.Interface.NameService.ContextName}-Errors")
                    {
                        tcs.TrySetResult(false);
                    }

                });

            event_store.Publish(trigger_as_raw_event);

            return await tcs.Task;
        }
    }
}
