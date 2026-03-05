namespace zombie_defense.Middleware
{
    public class ApiKeyMiddleware(RequestDelegate next,IConfiguration configuration)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue("X-API-KEY", out var apiKey))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("API key is missing.");
                return;
            }
            var configuredApiKey = configuration.GetValue<string>("ApiXKey");
            if (apiKey != configuredApiKey)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Invalid API key.");
                return;
            }
            await next(context);
        }
    }
}
