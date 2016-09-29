using System;
using LbspSOA;

namespace Booking.Domain
{
    public class World : IWorld
    {
        public readonly Guid world_id;
    }

    public class ParkingGuest
    {
        public readonly Guid guest_id;
    }
}
