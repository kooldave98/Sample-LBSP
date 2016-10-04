using System.Collections.Generic;
using System.Web.Http;
using Query.Interface;

namespace Gateway.Controllers
{
    public class GetAllHostsController : ApiController
    {
        [Route("api/get-all-hosts")]
        public IEnumerable<Host> Get()
        {
            //CurrentDbContext.Configuration.ProxyCreationEnabled = false;
            //AND- disable all those change tracking etc 

            return
                new Repository<Host>(new AppDbContext())
                .Entities;
        }
    }
}
