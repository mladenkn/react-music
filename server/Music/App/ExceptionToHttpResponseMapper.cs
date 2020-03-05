using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Music.App
{
    public class ExceptionToHttpResponseMapper : IActionFilter, IOrderedFilter
    {
        public int Order { get; set; } = int.MaxValue - 10;

        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is ApplicationException exception)
            {
                context.Result = new BadRequestObjectResult(exception.Message);
                context.ExceptionHandled = true;
            }
        }
    }

}
