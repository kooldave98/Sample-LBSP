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
        public async Task<string> Post(RegisterParkingHost trigger)
        {
            var response = await send(trigger);

            //Need to handle case when ===>
            //No handler defined for event with id

            if (response)
            {
                return "success";
            }


            return "an error occurred";
        }

        private async Task<bool> send(RegisterParkingHost trigger)
        {
            var trigger_as_raw_event = trigger.ToRawEvent();


            var event_store = new GESEventStore(Gateway.Interface.NameService.ContextName);
            //remember to conserve resources /avoid memory leaks 
            //=> event_store.unsubscribe_all();

            var tcs = new TaskCompletionSource<bool>();

            event_store
                .NewEvents(Query.Interface.NameService.ContextName)
                //.Where(re => re.raw_event.metadata.ToAnonType(new { parent_id = Guid.NewGuid()}).parent_id == trigger_as_raw_event.id)
                .Subscribe(re => {

                    var metadata =
                    re.raw_event.metadata.ToAnonType(new { parent_id = Guid.NewGuid() });
                    
                    if(metadata.parent_id == trigger_as_raw_event.id)
                    {
                        var po = 3;
                    }

                    if (re.raw_event.type == nameof(ParkingHostMaterialised) &&
                        re.raw_event.data.To<ParkingHostMaterialised>().host_id == trigger.host_id)
                    {
                        tcs.TrySetResult(true);
                    }
                    
                });

            //event_store
            //    .NewEvents($"{Gateway.Interface.NameService.ContextName}-Errors")
            //    .Where(re => re.raw_event.metadata.ToAnonType(new { parent_id = Guid.NewGuid() }).parent_id == trigger_as_raw_event.id)
            //    .Subscribe(re => {

                    
            //        tcs.TrySetResult(false);
                    

            //    });

            event_store.Publish(trigger_as_raw_event);

            return await tcs.Task;
        }
    }
}

/*
event_store
                .NewEvents(Query.Interface.NameService.ContextName, $"{Gateway.Interface.NameService.ContextName}-Errors")
                .Where(re => re.raw_event.metadata.ToAnonType(new { parent_id = Guid.NewGuid()}).parent_id == trigger_as_raw_event.id)
                .Subscribe(re => {

                    if(re.raw_event.type == nameof(ParkingHostMaterialised) &&
                        re.raw_event.data.To<ParkingHostMaterialised>().host_id == trigger.host_id)
                    {
                        tcs.TrySetResult(true);
                    }else
                    {
                        tcs.TrySetResult(false);
                    }
                    
                }); 
*/
