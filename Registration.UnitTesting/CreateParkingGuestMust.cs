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
    public class CreateParkingGuestMust
    {
        [TestMethod]
        public void execute_successfully()
        {
            var behaviour = new WillCreateParkingGuest();

            var response = behaviour.handle(new_request());

            Assert.AreEqual(1, response.events.Count());
        }

        private Request<RegistrationWorld, RegisterParkingGuest> new_request()
        {
            var trigger = new RegisterParkingGuest(Guid.NewGuid(), "kooldave98");
            
            return new Request<RegistrationWorld, RegisterParkingGuest>(RegistrationWorld.seed_world(), trigger);
        }
    }
}
