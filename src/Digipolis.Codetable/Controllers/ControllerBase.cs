﻿using Digipolis.Codetable.Business;
using Digipolis.Codetable.Models;
using Digipolis.Common.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System;

namespace Digipolis.Codetable.Controllers
{
    public abstract class ControllerBase : Controller
    {
        protected ControllerBase(ILogger<Controller> logger)
        {
            this.Logger = logger;
        }

        protected ILogger<Controller> Logger { get; private set; }

        protected virtual IActionResult BadRequestResult(ModelStateDictionary modelState)
        {
            // ToDo : does this have to be logged ?
            return new BadRequestObjectResult(modelState);
        }

        protected virtual IActionResult BadRequestResult(CodetableBusinessValidationException validationEx)
        {
            // ToDo : does this have to be logged ?
            var modelState = new ModelStateDictionary();
            foreach (var message in validationEx.Error.Messages)
            {
                modelState.AddModelError(String.Empty, message);
            }
            return new BadRequestObjectResult(modelState);
        }

        protected virtual IActionResult CreatedAtRouteResult(string routeName, object routeValues, object value)
        {
            return new CreatedAtRouteResult(routeName, routeValues, value);
        }

        protected virtual IActionResult InternalServerError(Exception ex, string message, params object[] args)
        {
            if (ex == null)
            {
                Logger.LogError(message, args);
                var error = new Error(message, args);
                return new ObjectResult(error) { StatusCode = 500 };
            }
            else
            {
                var errorMessage = String.Format(message, args);
                Logger.LogError("{0} : {1}", errorMessage, ExceptionHelper.GetAllToStrings(ex));
                var error = new Error("{0} : {1}", errorMessage, ex.Message);
                return new ObjectResult(error) { StatusCode = 500 };
            }
        }

        protected virtual IActionResult InternalServerError(Error error)
        {
            if (error == null) throw new ArgumentNullException("error", "error is null");
            foreach (var message in error.Messages)
                Logger.LogError(message);
            return new ObjectResult(error) { StatusCode = 500 };
        }

        protected virtual IActionResult OkResult()
        {
            return new StatusCodeResult(200);
        }

        protected virtual IActionResult OkResult(object value)
        {
            return new ObjectResult(value) { StatusCode = 200 };
        }

        protected virtual IActionResult NotFoundResult(string message, params object[] args)
        {
            Logger.LogWarning(message, args);
            return new ObjectResult(new Error(message, args)) { StatusCode = 404 };
        }
    }
}
