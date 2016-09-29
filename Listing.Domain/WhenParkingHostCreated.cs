using System;
using LbspSOA;
using Registration.Interface;

namespace Listing.Domain
{
    public class WhenParkingHostCreated : IBehavior<ParkingHostCreated, ListingWorld>
    {
        public Response<ListingWorld> handle(Request<ParkingHostCreated, ListingWorld> request)
        {
            throw new NotImplementedException();
        }
    }
}
