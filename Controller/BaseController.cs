using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentValidation;
using MeWhenAPI.Domain.DTO;
using MeWhenAPI.Domain.Exception;
using Microsoft.AspNetCore.Mvc;

namespace MeWhenAPI.Controller
{
    public class BaseController : ControllerBase
    {
        [NonAction]
        public IActionResult Data200(object? obj) => Ok(new BaseMessageDTO()
        {
            Data = obj,
            Message = "Successfully execute an operation 👍",
            StatusCode = StatusCodes.Status200OK
        });

        [NonAction]
        public IActionResult Created201(object obj) => Ok(new BaseMessageDTO()
        {
            Data = obj,
            Message = "Successfully insert new data 📜",
            StatusCode = StatusCodes.Status201Created
        });
    }
}