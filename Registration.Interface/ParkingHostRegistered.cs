﻿using System;
using CodeStructures;
using LbspSOA;

namespace Registration.Interface
{
    public class ParkingHostRegistered : ITrigger
    {
        public Guid host_id { get; private set; }
        public string username { get; private set; }

        public ParkingHostRegistered(Guid host_id, string username)
        {
            this.host_id = Guard.IsNotNull(host_id, nameof(host_id));
            this.username = Guard.IsNotNull(username, nameof(username));
        }
    }
}
