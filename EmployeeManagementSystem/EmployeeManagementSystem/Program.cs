// Dependency injection container

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
                                .RequireAuthenticatedUser()
                                .Build();

    options.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddDbContext<AppDBContext>(options=>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyConnectionStrings"));
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options=>
{
    
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;

}).AddEntityFrameworkStores<AppDBContext>();


//builder.Services.AddSingleton<IEmployeeRepository, MockEmployeeRepository>();
builder.Services.AddScoped<IEmployeeRepository, SqlEmployeeRepository>();


builder.Logging.ClearProviders();
builder.Host.UseNLog();

//Middlewares 
var app = builder.Build();

//Microsoft.AspNetCore.Hosting.IHostingEnvironment env = app.Services.GetService<Microsoft.AspNetCore.Hosting.IHostingEnvironment>(); 

app.UseStatusCodePagesWithReExecute("/Error/{0}");


//if(env.IsDevelopment())
//{
//    app.UseDeveloperExceptionPage();
//}

//static file middleware
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();
//app.MapControllerRoute("default", "{controller=home}/{action=index}/{id?}");

//attribute routing
app.MapControllers();
 
//app.MapGet("/", () => "Hello World!");
//app.MapGet("/Process", () =>
//{
//    throw new Exception("HAHA");
//    return System.Diagnostics.Process.GetCurrentProcess().ProcessName;
//});


app.Run();