using System;
using MeWhen.Application.Operation.Image.Query;
using Microsoft.AspNetCore.Mvc;

namespace MeWhen.API
{
    [Route("image")]
    public class ImageController : Controller
    {
        [HttpGet("get")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<string>))]
        public IActionResult GetImage([FromQuery] GetImageTagQuery data)
            => Json(GetImageTagQueryHandler.Handle(data));
        
    }
}
