using Example.Application.Dto;
using Example.Application.Helpers;
using Example.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;
using System.Net;

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
            dynamic erros = new ExpandoObject();
            erros.Error = new string[] { exception.Message };

            switch (exception)
            {
                case DbUpdateException:
                {
                    if (exception.InnerException != null && exception.InnerException.Message.Contains("Duplicate")) 
                    {
                        var key = exception.InnerException.Message.Split("key ")[1];
                        code = HttpStatusCode.BadRequest;
                        erros.Error = new string[] { $"Key {key} is unique" };
                    }
                    break;
                }

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

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = response.Status;
            return context.Response.WriteAsync(UtilHelper.Serialize(response));
        }
    }
}
