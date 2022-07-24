// Dependency injection container

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();


builder.Services.AddSingleton<IEmployeeRepository, MockEmployeeRepository>();

//Middlewares 
var app = builder.Build();

//Microsoft.AspNetCore.Hosting.IHostingEnvironment env = app.Services.GetService<Microsoft.AspNetCore.Hosting.IHostingEnvironment>(); 


//if(env.IsDevelopment())
//{
//    app.UseDeveloperExceptionPage();
//}
//static file middleware
app.UseStaticFiles();


//app.MapControllerRoute("default", "{controller=home}/{action=index}/{id?}");
app.MapControllers();
 
//app.MapGet("/", () => "Hello World!");
//app.MapGet("/Process", () =>
//{
//    throw new Exception("HAHA");
//    return System.Diagnostics.Process.GetCurrentProcess().ProcessName;
//});


app.Run();