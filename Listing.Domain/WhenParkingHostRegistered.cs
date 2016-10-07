using System;
using LbspSOA;
using Registration.Interface;

namespace Listing.Domain
{
    public class WhenParkingHostRegistered : ITriggerHandler<ListingWorld, ParkingHostRegistered>
    {
        public Response<ListingWorld> handle(Request<ListingWorld, ParkingHostRegistered> request)
        {
            throw new NotImplementedException();
        }

        public Type trigger_type()
        {
            throw new NotImplementedException();
        }
    }
}
