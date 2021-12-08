using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace API.Authentication.Attributes
{
    /// <summary>
    /// Validator action filter
    /// </summary>
    public class ValidatorActionFilter : IActionFilter
    {
        /// <summary>
        /// On action executing
        /// </summary>
        /// <param name="filterContext"></param>
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.ModelState.IsValid)
                filterContext.Result = new BadRequestObjectResult(filterContext.ModelState);
        }

        /// <summary>
        /// After action is executed
        /// </summary>
        /// <param name="filterContext"></param>
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }
    }
}
