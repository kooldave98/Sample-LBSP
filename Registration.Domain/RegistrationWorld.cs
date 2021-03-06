﻿using System;
using System.Collections.Generic;
using System.Linq;
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

        public static RegistrationWorld seed_world()
        {
            return new RegistrationWorld(Enumerable.Empty<ParkingHost>(), Enumerable.Empty<ParkingGuest>());
        }
    }

    public class ParkingHost
    {
        public readonly Guid host_id;
        public readonly string username;
        public readonly string email;
        
        public ParkingHost(Guid host_id, string username, string email)
        {
            this.host_id = Guard.IsNotNull(host_id, nameof(host_id));
            this.username = Guard.IsNotNull(username, nameof(username));
            this.email = Guard.IsNotNull(email, nameof(email));
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
