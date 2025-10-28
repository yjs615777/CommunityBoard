using CommunityBoard.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CommunityBoard.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // ModelState가 유효하지 않으면 → 400 Bad Request
            if (!context.ModelState.IsValid)
            {
                var details = context.ModelState
                    .Where(kv => kv.Value is { Errors.Count: > 0 })
                    .ToDictionary(
                        kv => kv.Key,
                        kv => kv.Value!.Errors
                            .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage)
                                ? "Invalid"
                                : e.ErrorMessage)
                            .ToArray()
                    );

                var error = Result<object>.Fail("validation_error", "입력값이 유효하지 않습니다.", details);
                context.Result = new BadRequestObjectResult(error);
                return;
            }

            await next(); // 다음 미들웨어 or 액션 실행
        }
    }
}
