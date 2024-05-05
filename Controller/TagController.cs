using System;
using MediatR;
using MeWhen.Service.Tag;
using Microsoft.AspNetCore.Mvc;

namespace MeWhen.Controller
{
    [Route("tag")]
    public class TagController(IMediator mediator) : BaseController
    {
        [HttpGet("search")]
        public async Task<IActionResult> Get([FromQuery] GetTagSuggestionQuery data) =>
            await Run(async () => Data200(await mediator.Send(data)));
    }
}
