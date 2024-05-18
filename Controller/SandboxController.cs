using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeWhen.Controller
{
    [Route("sandbox")]
    public class SandboxController(Supabase.Client _Supabase) : BaseController
    {
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Sandbox([FromForm] string username, [FromForm] string password)
        {
            var user = await _Supabase.Auth.SignIn(username, password);
            if (user == null) return BadRequest400("Failed");
            return Data200(user.AccessToken ?? "");
        }
    }
}
