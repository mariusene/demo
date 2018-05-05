using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;

namespace UCB.JapanCimzia.API
{
    public class InheritanceRouteProvider : DefaultDirectRouteProvider
    {
        protected override IReadOnlyList<IDirectRouteFactory> GetActionRouteFactories(HttpActionDescriptor actionDescriptor)
        {
            return actionDescriptor.GetCustomAttributes<IDirectRouteFactory>(inherit: true);
        }

        protected override string GetRoutePrefix(HttpControllerDescriptor controllerDescriptor)
        {
            var routePrefix = controllerDescriptor.GetCustomAttributes<IRoutePrefix>(inherit: true).FirstOrDefault();

            return routePrefix == null
                ? base.GetRoutePrefix(controllerDescriptor)
                : routePrefix.Prefix;
        }
    }
}