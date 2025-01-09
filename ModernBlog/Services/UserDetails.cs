using Microsoft.AspNetCore.Components.Authorization;
using Models;

namespace ModernBlog.Services;

public static class UserDetails
{
    public static async Task<UserDetailRecord> GetUserDetails(Task<AuthenticationState>? authenticationStateTask)
    {
        if (authenticationStateTask is null)
        {
            return new UserDetailRecord("Unknown", "Unknown");
        }
        AuthenticationState authState = await authenticationStateTask;
        var userName = authState.User?.Identity?.Name ?? "Unknown";
        var userId = authState.User?.FindFirst(c => c.Type.Contains("nameidentifier"))?.Value ?? "Unknown";
        return new UserDetailRecord(userName, userId);
    }
}