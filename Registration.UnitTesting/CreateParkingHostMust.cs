using System.Linq;
using LbspSOA;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
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
            var behaviour = new WillRegisterParkingHost();

            var response = behaviour.handle(new_request());

            Assert.AreEqual(1, response.events.Count());
        }

        [TestMethod]
        public void deserialise_trigger()
        {
            var trigger = new RegisterParkingHost("kooldave98", "kooldave98@hotmail.com");

            var as_json = JsonConvert.SerializeObject(trigger);

            var out_trigger = JsonConvert.DeserializeObject<RegisterParkingHost>(as_json);

            Assert.IsNotNull(out_trigger);

            Assert.AreEqual(trigger.host_id, out_trigger.host_id);
            Assert.AreEqual(trigger.username, out_trigger.username);
            Assert.AreEqual(trigger.email, out_trigger.email);
        }

        [TestMethod]
        public void correctly_add_new_host()
        {
            var behaviour = new WillRegisterParkingHost();

            var request = new_request();

            var response = behaviour.handle(request);

            Assert.IsTrue(
                response.world.hosts.Any(h => h.username == "kooldave98")
            );

            Assert.IsTrue(
                response.world.hosts.Any(h => h.email == "kooldave98@hotmail.com")
            );
        }

        private Request<RegistrationWorld, RegisterParkingHost> new_request()
        {
            var trigger = new RegisterParkingHost("kooldave98", "kooldave98@hotmail.com");
            
            return new Request<RegistrationWorld, RegisterParkingHost>(RegistrationWorld.seed_world(), trigger);
        }
    }
}
