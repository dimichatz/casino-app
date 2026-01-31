using CasinoAppBackend.Helpers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CasinoAppBackend.Core.Filters
{
    /// <summary>
    /// An action filter that sanitizes all incoming request DTOs by 
    /// trimming whitespace from all string properties (including nested objects and collections).
    /// Applied globally to ensure consistent input cleanup before any controller action executes.
    /// </summary>
    public class SanitizeInputFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            foreach (var arg in context.ActionArguments.Values)
            {
                if (arg != null)
                    InputSanitizer.TrimStringsDeep(arg);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
