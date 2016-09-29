using System;
using System.Collections.Generic;
using System.Data.Spatial;
using LbspSOA;

namespace Listing.Domain
{
    public class ListingWorld : IWorld
    {
        public readonly Guid world_id;

        public readonly Dictionary<Guid, ParkingHost> hosts;

        public ListingWorld(Guid world_id, Dictionary<Guid, ParkingHost> hosts)
        {
            this.world_id = world_id;
            this.hosts = hosts;
        }

        public ListingWorld()
        {
            world_id = Guid.NewGuid();
            hosts = new Dictionary<Guid, ParkingHost>();
        }
    }

    public class ParkingHost
    {
        public readonly Guid host_id;

        public readonly Dictionary<Guid, ParkingSpot> spots;

        public ParkingHost(Guid host_id, Dictionary<Guid, ParkingSpot> spots)
        {
            this.host_id = host_id;
            this.spots = spots;
        }

        public ParkingHost()
        { 
            this.host_id = Guid.NewGuid();
            this.spots = new Dictionary<Guid, ParkingSpot>();
        }
    }

    public class ParkingSpot
    {
        public readonly Guid spot_id;
        public readonly DbGeography geo_location;

        public ParkingSpot(Guid spot_id, DbGeography geo_location)
        {
            this.spot_id = spot_id;
            this.geo_location = geo_location;
        }
    }
}
