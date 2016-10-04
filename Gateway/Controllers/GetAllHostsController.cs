using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Query.Interface;

namespace Gateway.Controllers
{
    public class GetAllHostsController : ApiController
    {
        [Route("api/get-all-hosts")]
        public IEnumerable<HostView> Get()
        {
            //CurrentDbContext.Configuration.ProxyCreationEnabled = false;
            //AND- disable all those change tracking etc 

            return
                new Repository<Host>(new AppDbContext())
                .Entities
                .Select(h => new HostView() { HostID = h.HostID, Username = h.Username });
        }
    }

    public class HostView
    {
        public Guid HostID { get; set; }
        public string Username { get; set; }
    }
}
