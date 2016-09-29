using System;
using System.Collections.Generic;
using CodeStructures;
using LbspSOA;

namespace Registration.Domain
{
    public class RegistrationWorld : IWorld
    {
        public readonly IEnumerable<ParkingHost> hosts;
        public readonly IEnumerable<ParkingGuest> guests;

        public RegistrationWorld(IEnumerable<ParkingHost> hosts, IEnumerable<ParkingGuest> guests)
        {
            this.hosts = Guard.IsNotNull(hosts, nameof(hosts));
            this.guests = Guard.IsNotNull(guests, nameof(guests));
        }
    }

    public class ParkingHost
    {
        public readonly Guid host_id;
        public readonly string username;    
        
        public ParkingHost(Guid host_id, string username)
        {
            this.host_id = Guard.IsNotNull(host_id, nameof(host_id));
            this.username = Guard.IsNotNull(username, nameof(username));
        }
    }

    public class ParkingGuest
    {
        public readonly Guid guest_id;
        public readonly string username;

        public ParkingGuest(Guid guest_id, string username)
        {
            this.guest_id = Guard.IsNotNull(guest_id, nameof(guest_id));
            this.username = Guard.IsNotNull(username, nameof(username));
        }
    }
}
