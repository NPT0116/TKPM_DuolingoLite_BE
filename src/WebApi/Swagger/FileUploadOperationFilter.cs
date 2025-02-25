using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApi.Swagger
{
    public class FileUploadOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var fileParams = context.MethodInfo.GetParameters()
                .Where(p => p.ParameterType == typeof(IFormFile))
                .ToList();

            if (fileParams.Any())
            {
                // Xóa các parameter được thêm từ IFormFile
                operation.Parameters = operation.Parameters
                .Where(p => !fileParams.Any(fp => fp.Name == p.Name))
                .ToList();

                var properties = new Dictionary<string, OpenApiSchema>();
                var required = new HashSet<string>();

                // Sử dụng tên thực tế của từng parameter
                foreach (var param in fileParams)
                {
                    properties.Add(param.Name, new OpenApiSchema
                    {
                        Type = "string",
                        Format = "binary"
                    });
                    required.Add(param.Name);
                }

                operation.RequestBody = new OpenApiRequestBody
                {
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["multipart/form-data"] = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = "object",
                                Properties = properties,
                                Required = required
                            }
                        }
                    }
                };
            }
        }
    }
}
