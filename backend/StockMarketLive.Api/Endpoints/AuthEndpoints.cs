using StockMarketLive.Api.Filters;
using StockMarketLive.Application.DTOs.Auth;
using StockMarketLive.Application.Interfaces;
using System.Security.Claims;

namespace StockMarketLive.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth");

        // Public Endpoint
        group.MapPost("/login", async (IAuthService authService, LoginRequest request) =>
        {
            var result = await authService.LoginAsync(request.Username, request.Password);
            if (!result.IsSuccess) return Results.BadRequest(new { result.Error });
            return Results.Ok(new { Token = result.Value });
        }).AddEndpointFilter<ValidationFilter<LoginRequest>>();

        // Authenticated Endpoint (Herkes)
        group.MapGet("/me", [Microsoft.AspNetCore.Authorization.Authorize] async (ClaimsPrincipal user, IAuthService authService) =>
        {
            var userIdString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdString, out var userId)) return Results.Unauthorized();
            
            var result = await authService.GetProfileAsync(userId);
            if (!result.IsSuccess) return Results.NotFound(new { result.Error });
            return Results.Ok(result.Value);
        });

        // Admin Endpoints Group (Filtre ile korunuyor)
        var adminGroup = group.MapGroup("")
            .RequireAuthorization()
            .AddEndpointFilter<RequireAdminFilter>();

        adminGroup.MapPost("/register", async (IAuthService authService, RegisterRequest request) =>
        {
            var result = await authService.RegisterAsync(request.Username, request.Email, request.Password);
            if (!result.IsSuccess) return Results.BadRequest(new { result.Error });
            return Results.Ok(new { UserId = result.Value });
        }).AddEndpointFilter<ValidationFilter<RegisterRequest>>();

        adminGroup.MapGet("/users", async (IAuthService authService) =>
        {
            var usersResult = await authService.GetUsersAsync();
            return Results.Ok(usersResult.Value);
        });

        adminGroup.MapPost("/roles", async (IRoleService roleService, CreateRoleRequest request) =>
        {
            var result = await roleService.CreateRoleAsync(request.Name);
            if (!result.IsSuccess) return Results.BadRequest(new { result.Error });
            return Results.Ok(new { RoleId = result.Value });
        }).AddEndpointFilter<ValidationFilter<CreateRoleRequest>>();

        adminGroup.MapGet("/roles", async (IRoleService roleService) =>
        {
            var result = await roleService.GetRolesAsync();
            return Results.Ok(result.Value);
        });

        adminGroup.MapGet("/permissions", async (IRoleService roleService) =>
        {
            var result = await roleService.GetPermissionsAsync();
            return Results.Ok(result.Value);
        });

        adminGroup.MapPost("/users/{targetUserId:guid}/roles", async (IRoleService roleService, Guid targetUserId, AssignRoleRequest request) =>
        {
            var result = await roleService.AssignRoleToUserAsync(targetUserId, request.RoleId);
            if (!result.IsSuccess) return Results.BadRequest(new { result.Error });
            return Results.Ok();
        });

        adminGroup.MapPost("/roles/{roleId:guid}/permissions", async (IRoleService roleService, Guid roleId, AssignPermissionRequest request) =>
        {
            var result = await roleService.AssignPermissionToRoleAsync(roleId, request.PermissionId);
            if (!result.IsSuccess) return Results.BadRequest(new { result.Error });
            return Results.Ok();
        });
    }
}
