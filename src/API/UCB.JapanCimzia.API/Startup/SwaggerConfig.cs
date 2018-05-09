using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Filters;
using Swashbuckle.Application;
using Swashbuckle.Swagger;

namespace UCB.JapanCimzia.API
{
    public static class SwaggerConfig
    {
        public static void AddSwagger(this HttpConfiguration config)
        {
            config.EnableSwagger(c =>
            {
                c.SingleApiVersion("v1", "UCB.JapanCimzia.API");
                c.DescribeAllEnumsAsStrings();
                c.OperationFilter<AuthorizationHeaderParameter>();
                c.ApiKey("Authorization")
                    .Description(
                        "Standard Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"")
                    .Name("Authorization")
                    .In("header");
            })
            .EnableSwaggerUi(c =>
            {
                //c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
        }

        private class AuthorizationHeaderParameter : IOperationFilter
        {
            private readonly List<Type> _excludeTypes = new List<Type> { /*TODO: Controllers you want to exclude*/ };

            public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
            {
                var filterPipeline = apiDescription.ActionDescriptor.GetFilterPipeline();
                var isAuthorized = filterPipeline
                                                 .Select(filterInfo => filterInfo.Instance)
                                                 .Any(filter => filter is IAuthorizationFilter);
                var allowAnonymous = apiDescription.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();
                var isExcluded = _excludeTypes.Contains(apiDescription.ActionDescriptor.ControllerDescriptor
                    .ControllerType);

                if (isExcluded || !isAuthorized || allowAnonymous)
                    return;

                if (operation.parameters == null)
                    operation.parameters = new List<Parameter>();
                operation.parameters.Add(new Parameter
                {
                    name = "Authorization",
                    @default = "Bearer VALID_JWT_TOKEN_BASE64",
                    description = "JWT Bearer Token",
                    @in = "header",
                    type = "string",
                    required = true
                });
            }
        }
    }
}
