using AspNetCoreHero.ToastNotification;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using MyPaySlipLive.BLL.AddDefaultData;
using MyPaySlipLive.BLL.Interface;
using MyPaySlipLive.BLL.Service;
using MyPaySlipLive.CommonMethods;
using MyPaySlipLive.DAL;
using MyPaySlipLive.Extensions;

//await AddDefaultData.AddDefaultSuperAdmin();
//await AddDefaultData.AddDefaultRoles();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.LoginPath = "/Login";
            options.AccessDeniedPath = "/Login";
        });

builder.Services.AddNotyf(config =>
{
    config.DurationInSeconds = 6;
    config.IsDismissable = true;
    config.Position = NotyfPosition.BottomRight;
}
);
// Add services to the container.
builder.Services.AddControllersWithViews();

 
// Add DI

builder.Services.AddTransient<IJwt, JwtService>();
builder.Services.AddTransient<CommonExtensions>();
builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddTransient<IUser, UserService>();
builder.Services.AddTransient<PayslipDbContext>();
builder.Services.AddScoped<ICompany, CompanyService>();
builder.Services.AddScoped<IEmployee, EmployeeService>();
builder.Services.AddScoped<IBlog, BlogService>();


//AddDefaultData.AddDefaultRoles(); // Add Default Data in TblRole
//AddDefaultData.AddDefaultSuperAdmin(); // Add default Super Admin 


var app = builder.Build();

app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/Login") || context.Request.Path.StartsWithSegments("/Logout"))
    {
        context.Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, max-age=0";
        context.Response.Headers["Pragma"] = "no-cache";
        context.Response.Headers["Expires"] = "-1";
        context.Response.Headers["X-Frame-Options"] = "DENY";
        context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
    }
    await next();
});



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

 
app.UseAuthentication();
 app.UseAuthorization();

app.MapFallback(context => {
    context.Response.Redirect("/NotFound");
    return Task.CompletedTask;
});
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Home}/{Id?}");
  

});
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Account}/{action=Login}/{companyCode?}");

app.Run();
