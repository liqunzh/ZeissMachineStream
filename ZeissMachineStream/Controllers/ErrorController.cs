using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZeissMachineStream.Exceptions;
using ZeissMachineStream.Models;

namespace ZeissMachineStream.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<MachineStatusController> _logger;

        public ErrorController(ILogger<MachineStatusController> logger)
        {
            _logger = logger;
        }

        [HttpGet("/error")]
        public IActionResult HandleError()
        {
            var exceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>()!;

            if (exceptionHandlerFeature.Error is ResourceNotFoundException)
            {
                return NotFound(exceptionHandlerFeature.Error.Message);
            }
            if (exceptionHandlerFeature.Error is RequestValidationException)
            {
                return BadRequest(exceptionHandlerFeature.Error.Message);
            }

            return Problem(
                detail: exceptionHandlerFeature.Error.StackTrace,
                title: exceptionHandlerFeature.Error.Message);
        }
    }
}
