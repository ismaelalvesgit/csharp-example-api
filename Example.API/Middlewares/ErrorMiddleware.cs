using Example.Application.Dto;
using Example.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace Example.API.Middlewares
{
    public class ErrorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ErrorMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<ErrorMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static ExceptionDto GetResponseData(string requestId, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;
            var title = "Internal Server Error sorry 🤭";
            dynamic erros =  new {
                Error = new string[] { exception.Message },
            };

            switch (exception)
            {
                case NotFoundException:
                {
                    code = HttpStatusCode.NotFound;
                    title = "NotFound resource 👀";
                    break;
                }

                case BadRequestException:
                {
                    code = HttpStatusCode.BadRequest;
                    title = "oops !!! something is wrong check there  😩";
                    break;
                }
            }

            return new ExceptionDto()
            {
                TraceId = requestId,
                Status = (int)code,
                Title = title,
                Errors = erros
            };
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var requestId = context.Request.Headers["RequestId"].ToString();
            _logger.LogError($"RequestId: {requestId} Error: {exception}");
            var response = GetResponseData(requestId, exception);
            var jsonOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = response.Status;
            return context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
        }
    }
}
