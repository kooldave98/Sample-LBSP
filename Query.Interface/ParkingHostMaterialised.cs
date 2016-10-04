using System;
using CodeStructures;
using LbspSOA;

namespace Query.Interface
{
    public class ParkingHostMaterialised : ITrigger
    {
        public readonly Guid host_id;

        public ParkingHostMaterialised(Guid host_id)
        {
            this.host_id = Guard.IsNotNull(host_id, nameof(host_id));
        }
    }
}
