using System;
using System.Text;
using System.Web.Http;
using CodeStructures;
using LbspSOA;
using Registration.Interface;

namespace Gateway.Controllers
{
    public class CreateHostController : ApiController
    {
        [Route("api/create-parking-host")]
        public void Get(string username, string email)
        {
            var trigger = new CreateParkingHost(Guid.NewGuid(), username, email);

            var event_store = new GESEventStore(Gateway.Interface.NameService.ContextName);

            var outgoing_event = new RawEvent(Guid.NewGuid(),
                                                trigger.ToBytes(),
                                                null,
                                                nameof(CreateParkingHost)
                                                );


            event_store.SubscribeHenceForth(Query.Interface.NameService.ContextName, recorded_event => {
                if((Guid)recorded_event.raw_event.metadata.ToJsonDynamic().parent_id == outgoing_event.id)
                {

                }
            });

            event_store.Publish(outgoing_event.ToEnumerable());
        }
    }
}
