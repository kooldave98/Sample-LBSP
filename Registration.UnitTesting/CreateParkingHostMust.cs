using System;
using System.Linq;
using LbspSOA;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Registration.Domain;
using Registration.Interface;

namespace Registration.UnitTesting
{
    [TestClass]
    public class CreateParkingHostMust
    {
        [TestMethod]
        public void execute_successfully()
        {
            var behaviour = new WillCreateParkingHost();

            var response = behaviour.handle(new_request());

            Assert.AreEqual(1, response.events.Count());
        }

        [TestMethod]
        public void correctly_add_new_host()
        {
            var behaviour = new WillCreateParkingHost();

            var request = new_request();

            var response = behaviour.handle(request);

            Assert.IsTrue(
                response.world.hosts.Any(h => h.username == "kooldave98")
            );

            Assert.IsTrue(
                response.world.hosts.Any(h => h.email == "kooldave98@hotmail.com")
            );
        }

        private Request<RegistrationWorld> new_request()
        {
            var trigger = new CreateParkingHost(Guid.NewGuid(), "kooldave98", "kooldave98@hotmail.com");

            var as_json = JsonConvert.SerializeObject(trigger);

            dynamic as_dynamic = JToken.Parse(as_json);
            
            return new Request<RegistrationWorld>(RegistrationWorld.seed_world(), as_dynamic);
        }
    }
}
