using OBSWebsocketDotNet;
using SharpHook; // TODO: Remove SharpHook package as it didn't work for OBS
using StreamOverlay;
using StreamOverlay.Hubs;
using StreamOverlay.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddSingleton<EventSimulator>();
builder.Services.AddSingleton<ObsService>();
builder.Services.AddSingleton<OBSWebsocket>();
builder.Services.Configure<ObsOptions>(builder.Configuration.GetSection("Obs"));
builder.Services.AddSingleton<IObsService, ObsService>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<IObsService>());
builder.Services.AddSingleton<ObsDispatcher>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<ObsDispatcher>());

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
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
app.MapHub<ObsHub>("/obs-hub");

app.Run();