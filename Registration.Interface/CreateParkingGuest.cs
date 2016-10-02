using System;
using CodeStructures;
using LbspSOA;

namespace Registration.Interface
{
    public class CreateParkingGuest : ITrigger
    {
        public readonly Guid guest_id;
        public readonly string username;

        public CreateParkingGuest(Guid guest_id, string username)
        {
            this.guest_id = Guard.IsNotNull(guest_id, nameof(guest_id));
            this.username = Guard.IsNotNull(username, nameof(username));
        }

        public static IMaybe<CreateParkingGuest> from_dynamic(dynamic source)
        {
            return
            Safely.Do(() =>
                    new CreateParkingGuest(
                        (Guid)source.guest_id,
                        (string)source.username))
                        ;
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
}
