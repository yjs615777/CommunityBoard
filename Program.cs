using CommunityBoard.Data;
using CommunityBoard.Filters;
using CommunityBoard.Mapping;
using CommunityBoard.Middleware;
using CommunityBoard.Repositories;
using CommunityBoard.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
    
namespace CommunityBoard
{
    public class Program
    {
        public static async Task Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<ValidationFilter>(); // ← 전역 필터 등록
            });

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            // Swagger(OpenAPI) 관련 서비스 등록
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            // Repository 계층 의존성 주입 (DI 등록)
            builder.Services.AddScoped<IPostRepository, PostRepository>();
            builder.Services.AddScoped<ICommentRepository, CommentRepository>();
            builder.Services.AddScoped<ILikeRepository, LikeRepository>();
            // Service 계층 의존성 주입 (DI 등록)
            builder.Services.AddScoped<IPostService, PostService>();
            builder.Services.AddScoped<ICommentService, CommentService>();
            builder.Services.AddScoped<ILikeService, LikeService>();
            // AuthService를 DI(Container)에 등록
            builder.Services.AddScoped<IAuthService, AuthService>();

            
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(o =>
                {
                    // 로그인되지 않은 사용자가 보호된 페이지 접근 시 이동할 경로
                    o.LoginPath = "/Account/Login";

                    // 권한(Role) 부족 등 접근 거부 시 이동할 경로
                    o.AccessDeniedPath = "/Account/Denied";

                    // 쿠키 유효기간 연장 (사용 도중 만료 시간 갱신)
                    o.SlidingExpiration = true;

                    // 로그인 세션(쿠키) 유지 시간 설정 (8시간)
                    o.ExpireTimeSpan = TimeSpan.FromHours(8);
                });
            // 권한(Authorization) 정책 시스템 등록
            builder.Services.AddAuthorization();
            // MVC 컨트롤러 + 뷰 기능 등록 (RazorView 포함)
            builder.Services.AddControllersWithViews();

            // DbContext 등록 (DI)
            builder.Services.AddDbContext<CommunityContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

            builder.Services.AddAutoMapper(typeof(CommunityMappingProfile));

            var app = builder.Build();
            // Attribute Routing 기반 API 컨트롤러 활성화
            app.MapControllers();



            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CommunityBoard API v1");
                });
            }
  

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseMiddleware<GlobalExceptionMiddleware>(); //  전역 예외 처리 미들웨어 등록
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication(); // 인증 미들웨어: 쿠키/JWT 토큰 등 사용자 정보 복원
            app.UseAuthorization();  // 인가 미들웨어: [Authorize] 속성 정책 검사 수행

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<CommunityContext>();
                await SeedData.InitializeAsync(db);
            }


            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Landing}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
