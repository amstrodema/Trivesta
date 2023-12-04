using App.Services;
using AspNetCore.SEOHelper;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using trivesta;
using trivesta.Services;
using Trivesta.Data;

var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

string connectionString;
var env = builder.Environment;

try
{
    if (!env.IsDevelopment())
    {
        connectionString = config["ConnectionString"];
    }
    else
    {
        connectionString = config["ConnectionStringDev"];
    }
}
catch (Exception)
{
    connectionString = "";
}

builder.Services.AddDbContext<TrivestaContext>(
              o => o.UseSqlServer(connectionString)
               );
builder.Services.AddHttpClient();
builder.Services.AddSignalR();
builder.Services.AddDirectoryBrowser();
builder.Services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = long.MaxValue;
});

DependecyInjection.Register(builder.Services);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

try
{
    using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
    {
        scope.ServiceProvider.GetService<TrivestaContext>().Database.Migrate();
    }
}
catch (Exception e)
{
    FileService.WriteToFile("\n\n" + e, "ErrorLogs");
}

app.UseHttpsRedirection();

if (!env.IsDevelopment())
{
    try
    {
        app.UseStaticFiles(new StaticFileOptions
        {

            FileProvider = new PhysicalFileProvider(
                       Path.Combine(env.ContentRootPath, "Images")),
            RequestPath = "/trig"
        });

        app.UseStaticFiles(new StaticFileOptions
        {

            FileProvider = new PhysicalFileProvider(
            Path.Combine(env.ContentRootPath, "Videos")),
            RequestPath = "/vids"
        });

        app.UseStaticFiles(new StaticFileOptions
        {

            FileProvider = new PhysicalFileProvider(
             Path.Combine(env.ContentRootPath, "Documents")),
            RequestPath = "/docs"
        });
    }
    catch (Exception e)
    {
        FileService.WriteToFile("\n\n" + e, "ErrorLogs");
    }

}
else
{
    try
    {
        app.UseStaticFiles(new StaticFileOptions
        {

            FileProvider = new PhysicalFileProvider(
                       "C:\\Users\\ZYNXX\\Documents\\Projects\\2023\\TrendyCampus\\TrendyCampus\\bin\\Debug\\net6.0\\Images"),
            RequestPath = "/trig"
        });

        app.UseStaticFiles(new StaticFileOptions
        {

            FileProvider = new PhysicalFileProvider(
            "C:\\Users\\ZYNXX\\Documents\\Projects\\2023\\TrendyCampus\\TrendyCampus\\bin\\Debug\\net6.0\\Videos"),
            RequestPath = "/vids"
        });

        app.UseStaticFiles(new StaticFileOptions
        {

            FileProvider = new PhysicalFileProvider(
             "C:\\Users\\ZYNXX\\Documents\\Projects\\2023\\TrendyCampus\\TrendyCampus\\bin\\Debug\\net6.0\\Documents"),
            RequestPath = "/docs"
        });
    }
    catch (Exception e)
    {
        FileService.WriteToFile("\n\n" + e, "ErrorLogs");
    }
}

app.UseStaticFiles();
app.UseXMLSitemap(app.Environment.ContentRootPath);

app.UseRouting();

app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<DefaultHub>("/chatHub");

app.Run();
