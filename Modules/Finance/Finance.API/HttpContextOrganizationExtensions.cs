namespace Finance.API;

internal static class HttpContextOrganizationExtensions
{
    public static bool TryGetOrgAndUser(this HttpContext http, out Guid organizationId, out Guid userId)
    {
        organizationId = default;
        userId = default;
        if (!Guid.TryParse(http.Request.Headers["X-Organization-Id"].FirstOrDefault(), out organizationId))
        {
            return false;
        }

        return Guid.TryParse(http.Request.Headers["X-User-Id"].FirstOrDefault(), out userId);
    }

    public static bool TryGetTenantUser(this HttpContext http, out Guid tenantUserId) =>
        Guid.TryParse(http.Request.Headers["X-Tenant-User-Id"].FirstOrDefault(), out tenantUserId);
}
