using Example.Application.Helpers;
using System.Text;
using System.Text.Json;

namespace Example.API.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly string?[] _omitRouters;
        private readonly IHostEnvironment _environment;

        public LoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IConfiguration configuration, IHostEnvironment environment)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<LoggingMiddleware>();
            _environment = environment;
            _omitRouters = configuration.GetSection("IgnoreHostLogging").GetChildren().Select(x => x.Value).ToArray();
        }

        public async Task Invoke(HttpContext context)
        {
            var requestId = SetRequestId(context);

            if (!IsOmit(context))
            {
                await LogRequest(context, requestId);

                var originalResponseBody = context.Response.Body;

                using var responseBody = new MemoryStream();
                context.Response.Body = responseBody;
                await _next.Invoke(context);

                await LogResponse(context, requestId, responseBody, originalResponseBody);
            }
            else
            {
                await _next.Invoke(context);
            }
        }

        private async Task LogResponse(HttpContext context, string requestId, MemoryStream responseBody, Stream originalResponseBody)
        {
            var utcData = DateTime.UtcNow;
            var responseContent = new StringBuilder();
            responseContent.Append("Type: Response ");
            responseContent.Append($"ApplicationName: {_environment.ApplicationName} ");
            responseContent.Append($"RequestId: {requestId} ");
            responseContent.Append($"Time: {utcData.ToString()} ");
            responseContent.Append($"StatusCode: {context.Response.StatusCode} ");
            Dictionary<string, string> headers = new();
            foreach (var (headerKey, headerValue) in context.Response.Headers)
            {
                headers.Add(headerKey, headerValue.ToString());
            }
            responseContent.Append($"Headers: {UtilHelper.Serialize(headers)} ");
            responseBody.Position = 0;
            var content = await new StreamReader(responseBody).ReadToEndAsync();
            responseContent.Append($"Body: {content}");
            responseBody.Position = 0;
            await responseBody.CopyToAsync(originalResponseBody);
            context.Response.Body = originalResponseBody;

            _logger.LogInformation(responseContent.ToString());
        }

        private async Task LogRequest(HttpContext context, string requestId)
        {
            var requestContent = new StringBuilder();
            var utcData = DateTime.UtcNow;
            requestContent.Append("Type: Request ");
            requestContent.Append($"ApplicationName: {_environment.ApplicationName} ");
            requestContent.Append($"RequestId: {requestId} ");
            requestContent.Append($"Time: {utcData.ToString()} ");
            requestContent.Append($"Method: {context.Request.Method.ToUpper()} ");
            requestContent.Append($"Path: {context.Request.Path + context.Request.QueryString} ");
            Dictionary<string, string> headers = new();
            foreach (var (headerKey, headerValue) in context.Request.Headers)
            {
                headers.Add(headerKey, headerValue.ToString());
            }
            requestContent.Append($"Headers: {UtilHelper.Serialize(headers)} ");
            context.Request.EnableBuffering();
            var requestReader = new StreamReader(context.Request.Body);
            var content = await requestReader.ReadToEndAsync();
            requestContent.Append($"Body: {content}");

            _logger.LogInformation(requestContent.ToString());
            context.Request.Body.Position = 0;
        }

        private static string SetRequestId(HttpContext context)
        {
            var requestId = context.Request.Headers["RequestId"].ToString();

            if (string.IsNullOrEmpty(requestId))
            {
                requestId = Guid.NewGuid().ToString();
                context.Request.Headers.Add("RequestId", requestId);
            }

            context.Response.Headers.Add("RequestId", requestId);

            return requestId;
        }

        private bool IsOmit(HttpContext context)
        {
            return _omitRouters.Any(x => x == context.Request.Path);
        }
    }
}