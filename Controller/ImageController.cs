using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using MeWhen.Service.Image;
using Microsoft.AspNetCore.Mvc;

namespace MeWhen.Controller
{
    [Route("image")]
    public class ImageController(IMediator mediator) : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetImageQuery data) =>
            Data200(await mediator.Send(data));
    }
}