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


builder.Services.AddAuthentication().AddGoogle(options =>
{
    options.ClientId = "602647198473-aio3kn0oqrd4lele19tas7lopf4rdvk5.apps.googleusercontent.com";
    options.ClientSecret = "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";
}).AddTwitter(options=>
{
    options.ConsumerKey = "VIfexcQZM5uh0hfVA0afBB2ZL";
    options.ConsumerSecret = "M5htPbEtqbam2YKKlfkvXLb7rxXuuBqbEJLndM0iYxjoSwR2lf";
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

    options.AddPolicy("CannotEditYourself", policy => policy.AddRequirements(new ManageAdminRoleAndClaimsRequirement()));
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

    //need to confirm email
    options.SignIn.RequireConfirmedEmail = true;

    //use the custom token provider instead of the default one token provider
    options.Tokens.EmailConfirmationTokenProvider = "CustomEmailTokenProvider";

    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

}).AddEntityFrameworkStores<AppDBContext>().AddDefaultTokenProviders()
.AddTokenProvider<CustomEmailConfirmationTokenProvider<ApplicationUser>>("CustomEmailTokenProvider");

builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    //set the token life span to 5 hours
    options.TokenLifespan = TimeSpan.FromHours(5);
});

builder.Services.Configure<CustomEmailConfirmationTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromHours(1);
});

//builder.Services.AddSingleton<IEmployeeRepository, MockEmployeeRepository>();
builder.Services.AddScoped<IEmployeeRepository, SqlEmployeeRepository>();
builder.Services.AddSingleton<IAuthorizationHandler, CannotEditOwnRolesAndClaimsHandler>();
//builder.Services.AddSingleton<IAuthorizationHandler, MultihanderTest>();
builder.Services.AddSingleton<DataProtectionPurposStrings>();
//Dependency injection for email service
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Logging.ClearProviders();
builder.Host.UseNLog();

//Middlewares 
var app = builder.Build();

//Microsoft.AspNetCore.Hosting.IHostingEnvironment env = app.Services.GetService<Microsoft.AspNetCore.Hosting.IHostingEnvironment>(); 

//app.UseStatusCodePagesWithReExecute("/Error/{0}");


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