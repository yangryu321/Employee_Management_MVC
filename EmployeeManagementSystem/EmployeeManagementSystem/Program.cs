// Dependency injection container
var builder = WebApplication.CreateBuilder(args);

//Middlewares 
var app = builder.Build();

//static file middleware
app.UseStaticFiles();

app.MapGet("/", () => "Hello World!");
app.MapGet("/Process", () => System.Diagnostics.Process.GetCurrentProcess().ProcessName);
app.Run();