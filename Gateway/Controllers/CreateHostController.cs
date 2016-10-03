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

            event_store.Publish(
                new RawEvent(Guid.NewGuid(),
                            trigger.ToBytes(),
                            null,
                            nameof(CreateParkingHost)
                            ).ToEnumerable()
                        );
        }
    }
}
