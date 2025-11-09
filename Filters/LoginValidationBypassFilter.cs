using Microsoft.AspNetCore.Mvc.Filters;

namespace CommunityBoard.Filters
{
    /// 로그인 경로(/Account/Login)에서는 ValidationFilter 검증을 우회하도록 하는 필터
    public class LoginValidationBypassFilter : IActionFilter, IOrderedFilter
    {
        // 항상 제일 먼저 실행
        public int Order => int.MinValue;

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var path = context.HttpContext.Request.Path.Value?.ToLowerInvariant() ?? "";

            if (path.StartsWith("/account/login"))
            {
                // ValidationFilter가 이 요청을 건드리지 않도록 신호
                context.HttpContext.Items["SkipValidationFilter"] = true;
                
                // 모델 상태도 비워서 길이제한 등으로 막히지 않게
                context.ModelState.Clear();
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}