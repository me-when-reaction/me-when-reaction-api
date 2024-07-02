using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using MeWhenAPI.Service.App.Image;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeWhenAPI.Controller
{
    [Route("image")]
    public class ImageController(IMediator mediator) : BaseController
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get([FromQuery] GetImageQuery data) =>
            await Run(async () => Data200(await mediator.Send(data)));

        [HttpGet("{ID}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get([FromRoute] GetImageByIDQuery data) =>
            await Run(async () => Data200(await mediator.Send(data)));

        [HttpGet("random")]
        [AllowAnonymous]
        public async Task<IActionResult> Get([FromQuery] GetRandomImageQuery data) =>
            await Run(async () => Data200(await mediator.Send(data)));

        [HttpPost]
        public async Task<IActionResult> Insert([FromForm] InsertImageCommand data) =>
            await Run(async () => Data200(await mediator.Send(data)));

        [HttpPatch]
        public async Task<IActionResult> Insert([FromForm] UpdateImageCommand data) =>
            await Run(async () =>
            {
                await mediator.Send(data);
                return Data200("Successfuly update data");
            });

        [HttpDelete]
        public async Task<IActionResult> Insert([FromQuery] DeleteImageCommand data) =>
            await Run(async () =>
            {
                await mediator.Send(data);
                return Data200("Successfully delete data");
            });
    }
}