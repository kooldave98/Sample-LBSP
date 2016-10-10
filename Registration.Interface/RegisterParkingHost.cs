using System;
using CodeStructures;
using LbspSOA;

namespace Registration.Interface
{
    public class RegisterParkingHost : ITrigger
    {
        public readonly Guid host_id;
        public readonly string username;
        public readonly string email;

        public RegisterParkingHost(string username, string email, Guid host_id = default(Guid))
        {
            this.host_id = host_id == default(Guid) ? Guid.NewGuid() : host_id;
            this.username = Guard.IsNotNull(username, nameof(username));
            this.email = Guard.IsNotNull(email, nameof(email));
        }
    }

    public class ParkingHostUsernameTaken : AnErrorTrigger
    {
        public readonly string username;

        public ParkingHostUsernameTaken(string username) : base("")
        {
            this.username = Guard.IsNotNull(username, nameof(username));
        }
    }

    public class ParkingHostEmailTaken : AnErrorTrigger
    {
        public readonly string email;

        public ParkingHostEmailTaken(string email) : base("")
        {
            this.email = Guard.IsNotNull(email, nameof(email));
        }
    }

    public class ParkingHostIDTaken : AnErrorTrigger
    {
        public readonly Guid host_id;

        public ParkingHostIDTaken(Guid host_id) : base("")
        {
            this.host_id = Guard.IsNotNull(host_id, nameof(host_id));
        }
    }
}
