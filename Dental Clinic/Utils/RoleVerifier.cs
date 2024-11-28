using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Dental_Clinic.Enums;
using Dental_Clinic.Exceptions;

namespace Dental_Clinic.Utils;

public class RoleVerifier
{
    public static UserRole GetCurrentUserRole(HttpContext httpContext)
    {
        var roleClaim = httpContext.User.FindFirst(ClaimTypes.Role);
        if (roleClaim == null)
        {
            throw new UnauthorizedException("No role claim found");
        }
        if (Enum.TryParse<UserRole>(roleClaim.Value, out UserRole userRole))
        {
            return userRole;
        }
        throw new UnauthorizedException("Invalid role");
    }
    public static bool IsAdmin(HttpContext httpContext)
    {
        return GetCurrentUserRole(httpContext) == UserRole.Admin;
    }

    public static bool IsDentist(HttpContext httpContext)
    {
        return GetCurrentUserRole(httpContext) == UserRole.Dentist;
    }

    public static bool IsPatient(HttpContext httpContext)
    {
        return GetCurrentUserRole(httpContext) == UserRole.Patient;
    }
    public static void EnsureAdmin(HttpContext httpContext)
    {
        if (!IsAdmin(httpContext))
        {
            throw new UnauthorizedException("Admin access required");
        }
    }

    public static void EnsureDentist(HttpContext httpContext)
    {
        if (!IsDentist(httpContext))
        {
            throw new UnauthorizedException("Dentist access required");
        }
    }
    public static void EnsurePatient(HttpContext httpContext)
    {
        if (!IsPatient(httpContext))
        {
            throw new UnauthorizedException("Patient access required");
        }
    }
}
