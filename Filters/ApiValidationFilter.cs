using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CommunityBoard.Filters
{
    public sealed class ApiValidationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Items.TryGetValue("SkipValidationFilter", out var skip)
                && skip is bool b && b)
                return;

            //  ApiController 속성이 붙은 컨트롤러만 검사
            var isApiController =
                context.Controller.GetType().IsDefined(typeof(ApiControllerAttribute), inherit: true);

            if (!isApiController)
                return; // ← MVC 폼 요청은 그냥 통과

            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(kv => kv.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kv => kv.Key,
                        kv => kv.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                var payload = new
                {
                    success = false,
                    data = (object?)null,
                    error = new
                    {
                        code = "validation_error",
                        message = "입력값이 유효하지 않습니다.",
                        details = errors
                    }
                };

                context.Result = new JsonResult(payload)
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}