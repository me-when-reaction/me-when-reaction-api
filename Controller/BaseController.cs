using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MeWhen.Domain.Exception;
using Microsoft.AspNetCore.Mvc;

namespace MeWhen.Controller
{
    public class BaseController : ControllerBase
    {
        [NonAction]
        public IActionResult Data200(object obj) => Ok(obj);

        [NonAction]
        public IActionResult ServerError500(Exception e) => StatusCode(StatusCodes.Status500InternalServerError, e.Message);

        [NonAction]
        public IActionResult BadRequest400(BadRequestException e) => StatusCode(StatusCodes.Status400BadRequest, new {
            e.Message,
            ErrorList = e.ListError
        });

        [NonAction]
        public async Task<IActionResult> Run(Func<Task<IActionResult>> process)
        {
            try
            {
                return await process();
            }
            catch (BadRequestException e)
            {
                return BadRequest400(e);
            }
            catch (Exception ex)
            {
                return ServerError500(ex);
            }
        }
    }
}