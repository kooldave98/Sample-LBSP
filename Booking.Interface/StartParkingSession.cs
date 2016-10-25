using System;
using CodeStructures;
using LbspSOA;

namespace Booking.Interface
{
    public class StartParkingSession : ITrigger
    {
        public readonly Guid guest_id;
        public readonly Guid spot_id;        

        public StartParkingSession(Guid guest_id, Guid spot_id)
        {
            this.guest_id = Guard.IsNotNull(guest_id, nameof(guest_id));
            this.spot_id = Guard.IsNotNull(spot_id, nameof(spot_id));
        }
    }
}
