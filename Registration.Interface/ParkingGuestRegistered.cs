using System;
using CodeStructures;
using LbspSOA;

namespace Registration.Interface
{
    public class ParkingGuestRegistered : ITrigger
    {
        public Guid guest_id { get; set; }
        public string username { get; set; }

        public ParkingGuestRegistered(Guid guest_id, string username)
        {
            this.guest_id = Guard.IsNotNull(guest_id, nameof(guest_id));
            this.username = Guard.IsNotNull(username, nameof(username));
        }
    }
}
