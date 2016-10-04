using System;

namespace Query.Interface
{
    public class Host
    {
        public virtual int ID { get; set; }
        public virtual Guid HostID { get; set; }
        public virtual string Username { get; set; }
        public virtual string Email { get; set; }

    }
}
