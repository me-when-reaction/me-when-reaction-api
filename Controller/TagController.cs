using System;
using MediatR;
using MeWhenAPI.Service.App.Tag;
using Microsoft.AspNetCore.Mvc;

namespace MeWhenAPI.Controller
{
    [Route("tag")]
    public class TagController(IMediator mediator) : BaseController
    {
        [HttpGet("search")]
        public async Task<IActionResult> Get([FromQuery] GetTagSuggestionQuery data) =>
            await Run(async () => Data200(await mediator.Send(data)));

        [HttpPatch]
        public async Task<IActionResult> UpdateRating([FromForm] UpdateTagRatingCommand data) =>
            await Run(async () =>
            {
                await mediator.Send(data);
                return Data200("Successfully update this tag 👍");
            });
    }
}
