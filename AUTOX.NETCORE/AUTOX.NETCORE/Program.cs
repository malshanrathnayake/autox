using AUTOX.BLL;
using AUTOX.BLL.Interfaces;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = configuration.GetConnectionString("prod");

builder.Services.AddSingleton<IDatabaseService>(provider =>
{
    var dbService = new DatabaseServiceImpl();
    dbService.SetConnectionString(connectionString);
    return dbService;
});

builder.Services.AddSingleton<IBookiongService, BookiongServiceImpl>();
builder.Services.AddSingleton<IConfigurationService, ConfigurationServiceImpl>();
builder.Services.AddSingleton<IGoogleCalendarService, GoogleCalendarServiceImpl>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Autox}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
