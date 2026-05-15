using Microsoft.AspNetCore.Mvc;

namespace WebApp.Helpers
{
    public static class ControllerAuthExtensions
    {
        public static string? GetJwtToken(this Controller controller)
        {
            controller.HttpContext.Request.Cookies.TryGetValue(AuthConstants.AuthorizationCookie, out var jwt);
            return string.IsNullOrEmpty(jwt) ? null : jwt;
        }

        public static void SetJwtToken(this Controller controller, string token)
        {
            controller.Response.Cookies.Append(AuthConstants.AuthorizationCookie, token, new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });
        }

        public static void ClearJwtToken(this Controller controller)
        {
            controller.Response.Cookies.Delete(AuthConstants.AuthorizationCookie);
        }
    }
}
