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
            var context = new AppDbContext();

            context.Configuration.AutoDetectChangesEnabled = false;
            context.Configuration.LazyLoadingEnabled = false;
            context.Configuration.ProxyCreationEnabled = false;

            return
                new Repository<Host>(context)
                .Entities;
        }
    }
}