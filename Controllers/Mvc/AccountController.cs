using CommunityBoard.Contracts.Requests;
using CommunityBoard.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CommunityBoard.Controllers.Mvc;
[ApiExplorerSettings(IgnoreApi = true)]
public class AccountController(IAuthService authService, ILogger<AccountController> logger) : Controller
{
    private readonly IAuthService _authService = authService;
    private readonly ILogger<AccountController> _logger = logger;
    // 회원가입 폼
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register() => View();

    // 회원가입 처리
    [HttpPost]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterRequest req, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return View(req);
        try
        {
            var res = await _authService.RegisterAsync(req, ct);
            if (!res.Success)
            {
                _logger.LogWarning("회원가입 실패: Email={Email}, Message={Message}", req.Email, res.Error?.Message);
                ModelState.AddModelError(string.Empty, res.Error?.Message ?? "회원가입 실패");
                return View(req);
            }
            _logger.LogInformation("회원가입 성공: Email={Email}", req.Email);
            TempData["Success"] = "회원가입이 완료되었습니다. 로그인 해주세요.";
            return RedirectToAction(nameof(Login));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "회원가입 처리 중 예외 발생: Email={Email}", req.Email);
            ModelState.AddModelError(string.Empty, "시스템 오류가 발생했습니다. 잠시 후 다시 시도해주세요.");
            return View(req);
        }
    }

    // 로그인 폼
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login() => View();

    // 로그인 처리
    [HttpPost]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest req, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return View(req);
        try
        {
            var res = await _authService.ValidateUserAsync(req, ct);
            if (!res.Success || res.Data is null)
            {
                _logger.LogWarning("로그인 실패: Email={Email}", req.Email);
                ModelState.AddModelError(string.Empty, "이메일 또는 비밀번호가 올바르지 않습니다.");
                return View(req);
            }

            var user = res.Data;
            _logger.LogInformation("로그인 성공: UserId={UserId}, Email={Email}", user.Id, user.Email);

            // 쿠키용 Claims 생성    
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Name),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties { IsPersistent = true });

            TempData["Success"] = "로그인 성공!";
            return RedirectToAction("Index", "Landing");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "로그인 처리 중 예외 발생: Email={Email}", req.Email);
            ModelState.AddModelError(string.Empty, "시스템 오류가 발생했습니다. 잠시 후 다시 시도해주세요.");
            return View(req);
        }
    }

    // 로그아웃
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        try
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation("로그아웃 성공: User={User}", User.Identity?.Name);
            TempData["Success"] = "로그아웃되었습니다.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "로그아웃 처리 중 예외 발생: User={User}", User.Identity?.Name);
            TempData["Error"] = "로그아웃 중 오류가 발생했습니다.";
        }
        return RedirectToAction("Index", "Landing");
    }

    // 접근 거부
    [HttpGet]
    public IActionResult Denied() => View();
}


