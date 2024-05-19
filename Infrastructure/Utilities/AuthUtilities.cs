using System;
using System.Security.Claims;
using MeWhenAPI.Infrastructure.Helper;

namespace MeWhenAPI.Infrastructure.Utilities
{
    public interface IAuthUtilities
    {
        public Guid GetUserID();
        public bool IsAuthenticated();
    }

    public class AuthUtilities(IHttpContextAccessor _HTTP) : IAuthUtilities
    {
        public Guid GetUserID() =>
            _HTTP.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)?.ToGUID() ?? Guid.Empty;

        public bool IsAuthenticated() =>
            _HTTP.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
}
