using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using StockMarketLive.Application.Interfaces;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using StockMarketLive.Domain.Constants;

namespace StockMarketLive.Api.Filters;

public class RequireAdminFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var user = context.HttpContext.User;
        var authService = context.HttpContext.RequestServices.GetRequiredService<IAuthService>();
        
        var userIdString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out var currentUserId)) 
        {
            return Results.Unauthorized();
        }
        
        var profileResult = await authService.GetProfileAsync(currentUserId);
        if (!profileResult.IsSuccess || !profileResult.Value!.Roles.Any(r => r.Name == AppConstants.Roles.Admin)) 
        {
            return Results.Forbid();
        }

        return await next(context);
    }
}
