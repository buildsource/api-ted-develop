using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using TED.API.Response;

namespace TED.API.Filters;

public class ValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
            context.Result = new BadRequestObjectResult(new ApiResponse<List<Notification>>("Erro na Validação", context.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage.TrimEnd('.')).ToList()));
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}