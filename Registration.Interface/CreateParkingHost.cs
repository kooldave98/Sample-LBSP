using System;
using CodeStructures;
using LbspSOA;

namespace Registration.Interface
{
    public class CreateParkingHost : ITrigger
    {
        public readonly Guid host_id;
        public readonly string username;

        public CreateParkingHost(Guid host_id, string username)
        {
            this.host_id = Guard.IsNotNull(host_id, nameof(host_id));
            this.username = Guard.IsNotNull(username, nameof(username));
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

    public class ParkingHostIDTaken : IErrorTrigger
    {
        public readonly Guid host_id;

        public ParkingHostIDTaken(Guid host_id)
        {
            this.host_id = Guard.IsNotNull(host_id, nameof(host_id));
        }
    }
}
