using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MeWhen.Controller
{
    public class BaseController : ControllerBase
    {
        [NonAction]
        public IActionResult Data200(object obj) => Ok(obj);
    }
}