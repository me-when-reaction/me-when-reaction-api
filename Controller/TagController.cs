using System;
using MediatR;
using MeWhenAPI.Service.App.Tag;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeWhenAPI.Controller
{
    [Route("tag")]
    public class TagController(IMediator mediator) : BaseController
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetTagManagement([FromQuery] GetTagManagementQuery data) =>
            await Run(async () => Data200(await mediator.Send(data)));

        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSearch([FromQuery] GetTagSuggestionQuery data) =>
            await Run(async () => Data200(await mediator.Send(data)));

        [HttpPatch]
        public async Task<IActionResult> UpdateRating([FromForm] UpdateTagRatingCommand data) =>
            await Run(async () =>
            {
                await mediator.Send(data);
                return Data200("Successfully update this tag 👍");
            });

        [HttpDelete]
        public async Task<IActionResult> DeleteRating([FromQuery] DeleteTagCommand data) =>
            await Run(async () =>
            {
                await mediator.Send(data);
                return Data200("Successfully delete this tag 👍");
            });
    }
}
