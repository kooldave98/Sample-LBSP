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

        public RegisterParkingHost(Guid host_id, string username, string email)
        {            
            this.host_id = Guard.IsNotNull(host_id, nameof(host_id));
            this.username = Guard.IsNotNull(username, nameof(username));
            this.email = Guard.IsNotNull(email, nameof(email));
        }
    }

    public class ParkingHostUsernameTaken : IErrorTrigger
    {
        public readonly string username;

        public ParkingHostUsernameTaken(string username)
        {
            this.username = Guard.IsNotNull(username, nameof(username));
        }
    }

    public class ParkingHostEmailTaken : IErrorTrigger
    {
        public readonly string email;

        public ParkingHostEmailTaken(string email)
        {
            this.email = Guard.IsNotNull(email, nameof(email));
        }
    }

    public class ParkingHostIDTaken : IErrorTrigger
    {
        public readonly Guid host_id;

        public ParkingHostIDTaken(Guid host_id)
        {
            this.host_id = Guard.IsNotNull(host_id, nameof(host_id));
        }
    }
}
