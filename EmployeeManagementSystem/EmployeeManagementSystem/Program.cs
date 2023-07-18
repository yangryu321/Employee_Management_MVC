// Dependency injection container

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;
//logger
var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

var builder = WebApplication.CreateBuilder(args);

//authentication
builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
                                .RequireAuthenticatedUser()
                                .Build();

    options.Filters.Add(new AuthorizeFilter(policy));
});


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Create Role", policy =>policy.RequireClaim("Create Role", "true"));

    options.AddPolicy("Edit Role", policy => policy.RequireAssertion(context =>
        context.User.IsInRole("Admin") && context.User.HasClaim(claim => claim.Type == "Edit Role" && claim.Value == "true") ||
        context.User.IsInRole("Super Admin")
    ));

    options.AddPolicy("Delete User", policy => policy.RequireAssertion(context =>
      context.User.IsInRole("Admin") && context.User.HasClaim(claim => claim.Type == "Delete User" && claim.Value == "true") ||
      context.User.IsInRole("Super Admin")
      ));
});
 



//
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


//attribute routing
//app.MapControllers();

//convetional routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=home}/{action=index}/{id?}");


//app.MapGet("/", () => "Hello World!");
//app.MapGet("/Process", () =>
//{
//    throw new Exception("HAHA");
//    return System.Diagnostics.Process.GetCurrentProcess().ProcessName;
//});


app.Run();