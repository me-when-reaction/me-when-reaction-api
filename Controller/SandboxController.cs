using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeWhen.Controller
{
    [Route("sandbox")]
    public class SandboxController : BaseController
    {
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Sandbox([FromForm] string username, [FromForm] string password)
        {
            var user = await Program.Supabase.Auth.SignIn(username, password);
            if (user == null) return BadRequest400("Failed");
            return Data200(user.AccessToken ?? "");
        }

        [HttpGet]
        [Authorize]
        public IActionResult Test()
        {
            var a = User;
            return Data200(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "-");
        }

    }
}
