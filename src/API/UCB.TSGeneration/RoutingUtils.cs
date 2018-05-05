using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
#if NET_CORE
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Http;
#endif

namespace UCB.TSGeneration
{
    public class RoutingUtils
    {
        private const char UriSeparator = '/';

        private static readonly Type ControllerType =
#if NET_CORE
            typeof(Controller);
#else
            typeof(ApiController);
#endif

        private static readonly Type QueryAttributeType =
#if NET_CORE
            typeof(FromQueryAttribute);
#else
            typeof(FromUriAttribute);
#endif

        private static readonly Dictionary<string, HttpMethod> HttpMethodsByConvention = new Dictionary<string, HttpMethod>
        {
            { typeof(HttpGetAttribute).Name, HttpMethod.Get },
            { typeof(HttpPostAttribute).Name, HttpMethod.Post },
            { typeof(HttpPutAttribute).Name, HttpMethod.Put },
            { typeof(HttpDeleteAttribute).Name, HttpMethod.Delete },
            { typeof(HttpHeadAttribute).Name, HttpMethod.Head },
            { typeof(HttpOptionsAttribute).Name, HttpMethod.Options },
            { typeof(HttpPatchAttribute).Name, new HttpMethod("PATCH") },
        };

        public static Dictionary<string, RouteInfo> GetRoutes(Type contractType)
        {
#if NET_CORE
            var prefix = contractType.GetTypeInfo().GetCustomAttribute<RouteAttribute>().Template;
#else
            var prefix = contractType.GetCustomAttribute<RoutePrefixAttribute>().Prefix;
#endif
            var methods = contractType.GetMethods();
            var routes = new Dictionary<string, RouteInfo>();
            for (var i = 0; i < methods.Length; i++)
            {
                var method = methods[i];
                if (!IsValidMethod(method))
                {
                    continue;
                }

                var methodRoute = method.GetCustomAttributes<RouteAttribute>().OrderBy(t => t.Order).FirstOrDefault();
                var uri = string.Empty;
                if (prefix != null)
                {
                    uri = prefix;
                }
                if (methodRoute != null)
                {
                    uri = CombineRoutes(uri, methodRoute.Template);
                }
                uri = UriSeparator + uri.Trim(UriSeparator);

                var route = new RouteInfo { UrlTemplate = uri, Verb = GetVerb(method), Method = method };
                InitRouteParams(route);
                routes.Add(method.Name, route);
            }
            return routes;
        }

        public static string GetParamPlaceholder(string paramName)
        {
            return "{" + paramName + "}";
        }

        public static string CombineRoutes(string baseRoute, string relativeRoute)
        {
            return baseRoute.TrimEnd(UriSeparator) + UriSeparator + relativeRoute.TrimStart(UriSeparator);
        }

        public static string AddQueryParams(string baseRoute, List<string> queryParams)
        {
            var route = baseRoute;
            if (queryParams.Any())
            {
                route += "?" + string.Join("&", queryParams);
            }
            return route;
        }

        private static void InitRouteParams(RouteInfo route)
        {
            var parameters = route.Method.GetParameters();
            var routeParams = route.Params = new RouteParamInfo[parameters.Length];
            var isGet = route.Verb == HttpMethod.Get;
            for (var i = 0; i < parameters.Length; i++)
            {
                var param = parameters[i];
                var routeParam = routeParams[i] = new RouteParamInfo { Name = param.Name, ParameterInfo = param };
                var paramPlaceholder = GetParamPlaceholder(param.Name);
                if (route.UrlTemplate.IndexOf(paramPlaceholder, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    routeParam.IsUrlParam = true;
                }
                else if (isGet || param.GetCustomAttribute(QueryAttributeType) != null
                    || (IsPrimitiveType(param.ParameterType) && param.GetCustomAttribute<FromBodyAttribute>() == null))
                {
                    routeParam.IsQueryParam = true;
                }
                else
                {
                    routeParam.IsDataParam = true;
                }
            }
        }

        private static bool IsValidMethod(MethodInfo method)
        {
            return
                method.GetCustomAttribute<ObsoleteAttribute>() == null &&
                method.GetCustomAttribute<NonActionAttribute>() == null &&
                // not a normal method, e.g. a constructor or an event
                !method.IsSpecialName &&
                // is a method on Object, IHttpController, ApiController
                !method.GetBaseDefinition().DeclaringType.IsAssignableFrom(ControllerType);
        }

        private static HttpMethod GetVerb(MemberInfo method)
        {
            var attributes = method.GetCustomAttributes(true);
            foreach (var attr in attributes)
            {
                var attrType = attr.GetType().Name;
                if (HttpMethodsByConvention.ContainsKey(attrType))
                {
                    return HttpMethodsByConvention[attrType];
                }
            }
            foreach (var httpMethod in HttpMethodsByConvention.Values)
            {
                if (method.Name.StartsWith(httpMethod.Method, StringComparison.OrdinalIgnoreCase))
                {
                    return httpMethod;
                }
            }
            // yes, it's POST by default
            return HttpMethod.Post;
        }

        private static bool IsPrimitiveType(Type type)
        {
#if NET_CORE
            var typeInfo = type.GetTypeInfo();
#else
            var typeInfo = type;
#endif
            // check for nested nullable type
            if (typeInfo.IsGenericType
                && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return IsPrimitiveType(type.GetGenericArguments()[0]);
            }
            return typeInfo.IsPrimitive
              || typeInfo.IsEnum
              || type == typeof(string)
              || type == typeof(decimal)
              || type == typeof(DateTime)
              || type == typeof(DateTimeOffset);
        }
    }
}
