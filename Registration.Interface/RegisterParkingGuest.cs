﻿using System;
using CodeStructures;
using LbspSOA;

namespace Registration.Interface
{
    public class RegisterParkingGuest : ITrigger
    {
        public readonly Guid guest_id;
        public readonly string username;

        public RegisterParkingGuest(Guid guest_id, string username)
        {
            this.guest_id = Guard.IsNotNull(guest_id, nameof(guest_id));
            this.username = Guard.IsNotNull(username, nameof(username));
        }
    }

    public class ParkingGuestUsernameTaken : IErrorTrigger
    {
        public readonly string username;

        public ParkingGuestUsernameTaken(string username)
        {
            this.username = Guard.IsNotNull(username, nameof(username));
        }
    }

    public class ParkingGuestIDTaken : IErrorTrigger
    {
        public readonly Guid guest_id;

        public ParkingGuestIDTaken(Guid guest_id)
        {
            this.guest_id = Guard.IsNotNull(guest_id, nameof(guest_id));
        }
    }

    public class ParkingGuestEmailTaken : IErrorTrigger
    {
        public readonly string email;

        public ParkingGuestEmailTaken(string email)
        {
            this.email = Guard.IsNotNull(email, nameof(email));
        }
    }
}
