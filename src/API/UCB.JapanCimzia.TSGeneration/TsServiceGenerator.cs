using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using HandlebarsDotNet;
using TypeLite;
using TypeLite.TsModels;

namespace UCB.JapanCimzia.TSGeneration
{
    public class TsServiceGenerator
    {
        private readonly TsGenerator _modelGenerator;
        private readonly Func<object, string> _template;

        public TsServiceGenerator(TsGenerator modelGenerator = null)
        {
            _modelGenerator = modelGenerator ?? TypeScript.Definitions().ScriptGenerator;
            var templateSource = GetResource("TypeScriptService.hbs");
            var configuration = new HandlebarsConfiguration {TextEncoder = null};
            var handlebars = Handlebars.Create(configuration);
            _template = handlebars.Compile(templateSource);
        }

        public virtual StringBuilder Generate(Type controller)
        {
            var serviceName = GetServiceName(controller);
            var models = new List<TsServiceMethodModel>();
            var routes = RoutingUtils.GetRoutes(controller);
            var isAuthServiceRequired = false;
            var isHttpServiceRequired = false;
            foreach (var route in routes.Values)
            {
                var isUrlOnly = IsUrlOnly(route.Method.ReturnType);
                var addAccessToken = isUrlOnly && route.Method.GetCustomAttribute<AllowAnonymousAttribute>() == null;
                var model = new TsServiceMethodModel
                {
                    Uri = GetUri(route, addAccessToken, isUrlOnly),
                    IsUrlOnly = isUrlOnly,
                    MethodName = ToCamelCase(route.Method.Name),
                    InputParameterList = GetInputParametersList(route.Params),
                };
                if (!isUrlOnly)
                {
                    model.Verb = route.Verb.Method.ToLower(CultureInfo.InvariantCulture);
                    model.ResponseType = GetResponseType(route.Method.ReturnType);
                    model.DataParameterName = GetDataParameter(route);
                    isHttpServiceRequired = true;
                }
                models.Add(model);
                isAuthServiceRequired = isAuthServiceRequired || addAccessToken;
            }
            var service = _template(new TsServiceModel
            {
                ServiceName = serviceName,
                SourceTypeFullName = controller.FullName,
                TsGenProjectName = Assembly.GetExecutingAssembly().GetName().Name,
                Methods = models,
                IsHttpServiceRequired = isHttpServiceRequired,
                IsAuthServiceRequired = isAuthServiceRequired,
            });
            return new StringBuilder(service);
        }

        internal static string GetServiceName(Type controller)
        {
            return $"API{controller.Name.Replace("Controller", "Service")}";
        }

        internal static string GetServiceFileName(Type controller)
        {
            return $"API.{controller.Name.Replace("Controller", ".Service")}".ToLowerInvariant();
        }

        private string GetResponseType(Type type)
        {
            if (IsGenericTask(type))
            {
                return GetResponseType(type.GetGenericArguments()[0]);
            }
            if (type == typeof(void) || type == typeof(Task))
            {
                return string.Empty;
            }
            return  GetTsTypeName(type);
        }

        private static bool IsGenericTask(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>);
        }

        private string GetInputParametersList(IReadOnlyList<RouteParamInfo> paramInfos)
        {
            var result = new List<string>();
            for (var i = 0; i < paramInfos.Count; i++)
            {
                var param = paramInfos[i].ParameterInfo;
                var str = param.Name + ":" + GetTsTypeName(param.ParameterType);

                if (param.HasDefaultValue)
                {
                    if (param.DefaultValue == null)
                    {
                        str += " = null";
                    }
                    else
                    {
                        var defaultValue = param.DefaultValue.ToString();
                        if (param.ParameterType == typeof(string))
                        {
                            defaultValue = "'" + defaultValue + "'";
                        }
                        str += " = " + defaultValue;
                    }
                }
                result.Add(str);
            }
            return string.Join(", ", result);
        }

        private static object GetDataParameter(RouteInfo route)
        {
            var dataParam = route.Params.FirstOrDefault(t => t.IsDataParam);
            return dataParam == null ? null : dataParam.Name;
        }

        private string GetTsTypeName(Type type)
        {
            var tsType = (TsType)typeof(TsType).GetMethod("Create", BindingFlags.Static | BindingFlags.NonPublic)
                .Invoke(null, new object[] { type });

            var name = _modelGenerator.GetFullyQualifiedTypeName(tsType);

            if (tsType is TsCollection) name += "[]";
            return name;
        }

        private static string GetUri(RouteInfo route, bool addAccessToken, bool addCultureInfo)
        {
            var uri = route.UrlTemplate;
            var queryParams = new List<string>();
            foreach (var param in route.Params)
            {
                if (param.IsUrlParam)
                {
                    var paramPlaceholder = RoutingUtils.GetParamPlaceholder(param.Name);
                    uri = uri.Replace(paramPlaceholder, "$" + paramPlaceholder);
                }
                else if (param.IsQueryParam)
                {
                    queryParams.Add(param.ParameterInfo.ParameterType != typeof(string)
                    ? param.Name + "=${" + param.Name + "}"
                    : param.Name + "=${encodeURIComponent(" + param.Name + ")}");
                }
            }

            //if (addAccessToken)
            //{
            //    queryParams.Add(Platform.Authorization.OAuth.Constants.Request.AccessToken + "=${encodeURIComponent(this.accessToken)}");
            //}

            return RoutingUtils.AddQueryParams(uri, queryParams);
        }

        private static bool IsUrlOnly(Type type)
        {
            if (IsGenericTask(type))
            {
                return IsUrlOnly(type.GetGenericArguments()[0]);
            }
            return type != typeof(void) && type.IsAssignableFrom(typeof(HttpResponseMessage));
        }

        private static string GetResource(string resourceName)
        {
            var assembly = typeof(TsServiceGenerator).Assembly;
            var fullName = assembly.GetManifestResourceNames().FirstOrDefault(t => t.EndsWith(resourceName, StringComparison.Ordinal));
            using (var stream = assembly.GetManifestResourceStream(fullName))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        private static string ToCamelCase(string input)
        {
            return char.ToLower(input[0]) + input.Substring(1);
        }
    }
}
