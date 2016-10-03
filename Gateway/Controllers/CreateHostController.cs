using System;
using System.Text;
using System.Web.Http;
using CodeStructures;
using LbspSOA;
using Newtonsoft.Json;
using Registration.Interface;

namespace Gateway.Controllers
{
    public class CreateHostController : ApiController
    {
        [Route("api/create-parking-host")]
        public void Get(string username, string email)
        {
            var trigger = new CreateParkingHost(Guid.NewGuid(), username, email);

            var event_store = new GESEventStore("Gateway");

            event_store.PublishResponse(
                new RawEvent(Guid.NewGuid(),
                            Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(trigger)),
                            null,
                            nameof(CreateParkingHost)
                            ).ToEnumerable()
                        );
        }
    }
}
