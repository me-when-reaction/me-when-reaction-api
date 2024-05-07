using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentValidation;
using MeWhen.Domain.DTO;
using MeWhen.Domain.Exception;
using Microsoft.AspNetCore.Mvc;

namespace MeWhen.Controller
{
    public class BaseController : ControllerBase
    {
        [NonAction]
        public IActionResult Data200(object obj) => Ok(new BaseMessageDTO()
        {
            Data = obj,
            StatusCode = StatusCodes.Status200OK
        });

        [NonAction]
        public IActionResult Created201() => Ok(new BaseMessageDTO()
        {
            Data = "Successfully insert new data",
            StatusCode = StatusCodes.Status201Created
        });

        [NonAction]
        public IActionResult ServerError500(Exception e) => StatusCode(StatusCodes.Status500InternalServerError, new BaseMessageDTO()
        {
            Data = e.Message,
            StatusCode = StatusCodes.Status400BadRequest
        });

        [NonAction]
        public IActionResult BadRequest400(object o) => StatusCode(StatusCodes.Status400BadRequest, new BaseMessageDTO()
        {
            Data = o,
            StatusCode = StatusCodes.Status400BadRequest
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
            catch (ValidationException e)
            {
                return BadRequest400(e.Errors.Select(x => x.ErrorMessage).ToList());
            }
            catch (Exception ex)
            {
                return ServerError500(ex);
            }
        }
    }
}