namespace GameShopAPI.Middlewares;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var headerAndPayload = context.Request.Cookies["headerAndPayload"];
        var signature = context.Request.Cookies["signature"];

        if (headerAndPayload != null && signature != null &&
            !context.Request.Headers.ContainsKey("Authorization"))
        {
            var jwtToken = headerAndPayload + "." + signature;
            context.Request.Headers.Add("Authorization", "Bearer " + jwtToken);
        }

        await _next(context);
    }
}
