using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EmployeeManagement.Controllers
{
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> logger;
        public ErrorController(ILogger<ErrorController> logger)
        {
            this.logger = logger;
        }
        // GET: /<controller>/
        [Route("Error/{StatusCode}")]
        public IActionResult HttpSttausCodeHandler(int statusCode)
        {
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = "Sorry, the resoure you requested could not be found";
                    logger.LogWarning($"404 Error Occured. Path = {statusCodeResult.OriginalPath}" + 
                        $"and QueryString = {statusCodeResult.OriginalQueryString}" );

                    break;

                case 500:
                    ViewBag.ErrorMessage = "Sorry, the resoure you requested could not be found";

                    break;
            }

            return View("NotFound");
        }

        [Route("Error")]
        [AllowAnonymous]
        public IActionResult Error()
        {
            // Retrive Exception Details
            var exceptioDetails = 
                HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            ViewBag.ExceptionPath = exceptioDetails.Path;
            ViewBag.ExceptionMessage = exceptioDetails.Error.Message;
            ViewBag.ExceptionStackTrace = exceptioDetails.Error.StackTrace;

            logger.LogError($"This path {exceptioDetails.Path} threw an exception" +
                $"{exceptioDetails.Error}"); 


            return View("Error");
        }
    }
}
