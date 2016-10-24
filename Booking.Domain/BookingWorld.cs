﻿using System;
using System.Collections.Generic;
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
        public readonly IEnumerable<ParkingSpot> spots;
        public readonly IEnumerable<ParkingGuest> guests;
        public readonly IEnumerable<LiveSession> liveSessions;
        public readonly IEnumerable<CompletedSession> completedSessions;


        public BookingWorld(IEnumerable<ParkingSpot> spots, 
                            IEnumerable<ParkingGuest> guests, 
                            IEnumerable<LiveSession> liveSessions, 
                            IEnumerable<CompletedSession> completedSessions)
        {
            this.spots = Guard.IsNotNull(spots, nameof(spots));
            this.guests = Guard.IsNotNull(guests, nameof(guests));
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
        public readonly Guid host_id;
        //location

        public ParkingSpot(Guid host_id)
        {
            this.host_id = Guard.IsNotNull(host_id, nameof(host_id));
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
