using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace LetsDisc.Web.Host.Startup
{
    public class FileUploadOperation : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.OperationId.ToLower().Contains("upload"))
            {
                operation.Parameters.Clear();
                operation.Parameters.Add(new NonBodyParameter
                {
                    Name = "uploadedFile",
                    In = "formData",
                    Description = "Upload File",
                    Required = true,
                    Type = "file"
                });
                operation.Parameters.Add(new NonBodyParameter
                {
                    Name = "userId",
                    In = "query",
                    Description = "",
                    Required = true,
                    Type = "long"
                });
                operation.Consumes.Add("multipart/form-data");
            }
        }
    }
}
