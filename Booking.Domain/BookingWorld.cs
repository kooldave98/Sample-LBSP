using System;
using System.Collections.Generic;
using System.Data.Spatial;
using System.Linq;
using CodeStructures;
using LbspSOA;


/*
 * Behaviours
 * 1. start session
 * 2. stop session
 * 3. timeout session ?
 */


namespace Booking.Domain
{
    public class BookingWorld : IWorld
    {
        public readonly IEnumerable<ParkingSpot> known_spots;
        public readonly IEnumerable<ParkingGuest> known_guests;
        public readonly IEnumerable<LiveSession> liveSessions;
        public readonly IEnumerable<CompletedSession> completedSessions;


        public BookingWorld(IEnumerable<ParkingSpot> spots, 
                            IEnumerable<ParkingGuest> guests, 
                            IEnumerable<LiveSession> liveSessions, 
                            IEnumerable<CompletedSession> completedSessions)
        {
            this.known_spots = Guard.IsNotNull(spots, nameof(spots));
            this.known_guests = Guard.IsNotNull(guests, nameof(guests));
            this.liveSessions = Guard.IsNotNull(liveSessions, nameof(liveSessions));
            this.completedSessions = Guard.IsNotNull(completedSessions, nameof(completedSessions));
        }

        public static BookingWorld seed_world()
        {
            return new BookingWorld(Enumerable.Empty<ParkingSpot>(), 
                                    Enumerable.Empty<ParkingGuest>(),
                                    Enumerable.Empty<LiveSession>(),
                                    Enumerable.Empty<CompletedSession>());
        }
    }

    public class LiveSession
    {
        public readonly ParkingSpot spot;
        public readonly ParkingGuest guest;
        public readonly DateTime start_time;

        public LiveSession(ParkingSpot spot, ParkingGuest guest)
        {
            this.spot = Guard.IsNotNull(spot, nameof(spot));
            this.guest = Guard.IsNotNull(guest, nameof(guest));
            this.start_time = DateTime.UtcNow.Date;
        }
    }

    public class CompletedSession
    {
        public readonly ParkingSpot spot;
        public readonly ParkingGuest guest;
        public readonly DateTime start_time;
        public readonly DateTime end_time;

        public CompletedSession(LiveSession live_session)
        {
            this.spot = Guard.IsNotNull(live_session.spot, nameof(live_session.spot));
            this.guest = Guard.IsNotNull(live_session.guest, nameof(live_session.guest));
            this.start_time = Guard.IsNotNull(live_session.start_time, nameof(live_session.start_time));
            this.end_time = DateTime.UtcNow.Date;
        }
    }

    public class ParkingSpot
    {
        public readonly Guid spot_id;        
        public readonly DbGeography location;
        //location
        public readonly Guid host_id;

        public ParkingSpot(Guid spot_id, Guid host_id, DbGeography location)
        {
            this.spot_id = Guard.IsNotNull(spot_id, nameof(spot_id));
            this.host_id = Guard.IsNotNull(host_id, nameof(host_id));
            this.location = Guard.IsNotNull(location, nameof(location));
        }
    }

    public class ParkingGuest
    {
        public readonly Guid guest_id;

        public ParkingGuest(Guid guest_id)
        {
            this.guest_id = Guard.IsNotNull(guest_id, nameof(guest_id));
        }
    }
}
