using System;
using System.Linq;
using Booking.Interface;
using LbspSOA;

namespace Booking.Domain
{
    public class WillStartParkingSession : ITriggerHandler<BookingWorld, StartParkingSession>
    {
        public Response<BookingWorld> handle(Request<BookingWorld, StartParkingSession> request)
        {
            throw new NotImplementedException();

            var world = request.world;

            if(world.liveSessions.Any(s=>s.spot.spot_id == request.trigger.spot_id))
            {
                //Fail !!!
                //spot is currently in use
            }

            if(!world.known_spots.Any(s=>s.spot_id == request.trigger.spot_id))
            {
                //Fail !!!
                //Spot doesn't exist
            }

            if (!world.known_guests.Any(s => s.guest_id == request.trigger.guest_id))
            {
                //Fail !!!
                //Guest doesn't exist
            }


        }
    }
}
