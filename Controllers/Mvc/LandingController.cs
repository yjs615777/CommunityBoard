using Microsoft.AspNetCore.Mvc;

namespace CommunityBoard.Controllers.Mvc
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class LandingController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            return View(); // Views/Landing/Index.cshtml
        }
    }

}
