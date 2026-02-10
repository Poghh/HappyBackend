using Happy.Backend.Api.Constants;
using Happy.Backend.Api.Models;
using Happy.Backend.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Happy.Backend.Api.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class JwtAuthorizeAttribute : Attribute, IAsyncActionFilter
{
    private const string HeaderName = "Authorization";
    private const string BearerPrefix = "Bearer ";

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(HeaderName, out var authHeader))
        {
            context.Result = new UnauthorizedObjectResult(new CommonResponseModel<object>(
                CommonResponseConstants.StatusUnauthorized,
                null,
                CommonMessageConstants.MissingAuthorizationHeader));
            return;
        }

        var value = authHeader.ToString();
        if (!value.StartsWith(BearerPrefix, StringComparison.OrdinalIgnoreCase))
        {
            context.Result = new UnauthorizedObjectResult(new CommonResponseModel<object>(
                CommonResponseConstants.StatusUnauthorized,
                null,
                CommonMessageConstants.InvalidAuthorizationScheme));
            return;
        }

        var token = value.Substring(BearerPrefix.Length).Trim();

        var jwtService = context.HttpContext.RequestServices.GetRequiredService<IJwtService>();
        var claims = jwtService.ValidateToken(token);

        if (claims == null)
        {
            context.Result = new UnauthorizedObjectResult(new CommonResponseModel<object>(
                CommonResponseConstants.StatusUnauthorized,
                null,
                CommonMessageConstants.InvalidOrExpiredToken));
            return;
        }

        context.HttpContext.Items["AppName"] = claims.AppName;
        context.HttpContext.Items["Phone"] = claims.Phone;

        await next();
    }
}
