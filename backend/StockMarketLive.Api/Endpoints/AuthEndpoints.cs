using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using StockMarketLive.Application.DTOs.Auth;
using StockMarketLive.Application.Interfaces;
using System;

namespace StockMarketLive.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth");

        group.MapPost("/login", async (IAuthService authService, LoginRequest request) =>
        {
            var result = await authService.LoginAsync(request.Username, request.Password);
            if (!result.IsSuccess) return Results.BadRequest(new { Error = result.Error });
            return Results.Ok(result.Value);
        });

        group.MapPost("/register", [Microsoft.AspNetCore.Authorization.Authorize] async (System.Security.Claims.ClaimsPrincipal userPrincipal, IAuthService authService, RegisterRequest request) =>
        {
            var userIdString = userPrincipal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdString, out var userId)) return Results.Unauthorized();
            
            var profileResult = await authService.GetProfileAsync(userId);
            if (!profileResult.IsSuccess || profileResult.Value?.IsAdmin != true) return Results.Forbid();

            var result = await authService.RegisterAsync(request.Username, request.Email, request.Password);
            if (!result.IsSuccess) return Results.BadRequest(new { Error = result.Error });
            return Results.Ok(new { UserId = result.Value });
        });

        group.MapGet("/me", [Microsoft.AspNetCore.Authorization.Authorize] async (System.Security.Claims.ClaimsPrincipal user, IAuthService authService) =>
        {
            var userIdString = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdString, out var userId)) return Results.Unauthorized();
            
            var result = await authService.GetProfileAsync(userId);
            if (!result.IsSuccess) return Results.NotFound(new { Error = result.Error });
            return Results.Ok(result.Value);
        });

        group.MapGet("/users", [Microsoft.AspNetCore.Authorization.Authorize] async (System.Security.Claims.ClaimsPrincipal user, IAuthService authService) =>
        {
            var userIdString = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdString, out var userId)) return Results.Unauthorized();
            
            // Kullanıcının admin olup olmadığını doğrula (güvenlik)
            var profileResult = await authService.GetProfileAsync(userId);
            if (!profileResult.IsSuccess || profileResult.Value?.IsAdmin != true) return Results.Forbid();
            
            var usersResult = await authService.GetUsersAsync();
            return Results.Ok(usersResult.Value);
        });
    }
}
