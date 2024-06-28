using SE172266.ProductManagement.Repo.Model;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace SE172266.ProductManagement.API.Middleware
{
    public class ApiResponseMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiResponseMiddleware> _logger;

        public ApiResponseMiddleware(RequestDelegate next, ILogger<ApiResponseMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;

            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;

                try
                {
                    await _next(context);

                    if (context.Response.StatusCode != StatusCodes.Status204NoContent)
                    {
                        responseBody.Seek(0, SeekOrigin.Begin);
                        var responseContent = await new StreamReader(responseBody).ReadToEndAsync();

                        if (context.Response.ContentType != null && context.Response.ContentType.Contains("application/json"))
                        {
                            object responseObject = null;
                            try
                            {
                                responseObject = JsonSerializer.Deserialize<object>(responseContent);
                            }
                            catch (JsonException jsonEx)
                            {
                                _logger.LogError(jsonEx, "JSON deserialization failed.");
                            }

                            var apiResponse = new ApiResponse<object>
                            {
                                StatusCode = context.Response.StatusCode,
                                Message = GetDefaultMessageForStatusCode(context.Response.StatusCode),
                                Data = responseObject
                            };

                            var jsonResponse = JsonSerializer.Serialize(apiResponse, new JsonSerializerOptions
                            {
                                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                            });

                            context.Response.ContentType = "application/json";
                            context.Response.ContentLength = Encoding.UTF8.GetBytes(jsonResponse).Length;
                            context.Response.Body = originalBodyStream;
                            await context.Response.WriteAsync(jsonResponse);
                        }
                        else
                        {
                            responseBody.Seek(0, SeekOrigin.Begin);
                            await responseBody.CopyToAsync(originalBodyStream);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An unhandled exception occurred while processing the request.");
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    var apiResponse = new ApiResponse<object>
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = GetDefaultMessageForStatusCode(context.Response.StatusCode),
                        Data = null
                    };
                    var jsonResponse = JsonSerializer.Serialize(apiResponse, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });
                    context.Response.ContentType = "application/json";
                    context.Response.ContentLength = Encoding.UTF8.GetBytes(jsonResponse).Length;
                    await context.Response.WriteAsync(jsonResponse);
                }
                finally
                {
                    context.Response.Body = originalBodyStream;
                }
            }
        }

        private string GetDefaultMessageForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                StatusCodes.Status200OK => "OK",
                StatusCodes.Status400BadRequest => "Bad Request",
                StatusCodes.Status401Unauthorized => "Unauthorized",
                StatusCodes.Status404NotFound => "Not Found",
                StatusCodes.Status500InternalServerError => "Internal Server Error",
                _ => "An error occurred"
            };
        }
    }
}
