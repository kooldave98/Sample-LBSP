using System;
using CodeStructures;
using LbspSOA;

namespace Registration.Interface
{
    public class CreateParkingHost : ITrigger
    {
        public readonly Guid host_id;
        public readonly string username;
        public readonly string email;

        public CreateParkingHost(Guid host_id, string username, string email)
        {
            this.host_id = Guard.IsNotNull(host_id, nameof(host_id));
            this.username = Guard.IsNotNull(username, nameof(username));
            this.email = Guard.IsNotNull(email, nameof(email));
        }

        public static IMaybe<CreateParkingHost> from_dynamic(dynamic source)
        {
            return
            Safely.Do(() =>
                    new CreateParkingHost(
                        (Guid)source.host_id,
                        (string)source.username,
                        (string)source.email))
                        ;
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
