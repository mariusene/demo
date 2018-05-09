using System.Web.Http;
using System.Web.Http.Cors;

namespace UCB.JapanCimzia.API.Controllers
{

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/auth-orders")]
    [Authorize]
    public class AuthorizedOrdersController : OrdersController
    {
    }
}