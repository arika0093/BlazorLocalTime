using BlazorLocalTime;
using BlazorLocalTimeSample;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;
using MudBlazor.Services;

// Set the local time zone to UTC (for sample purpose)
LocalTimeZoneOverwrite.UseUtc();

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Add localtime service
builder.Services.AddBlazorLocalTimeService();

// for sample
builder.Services.AddFluentUIComponents();
builder.Services.AddMudServices();
builder.Services.AddAntDesign();

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress),
});

await builder.Build().RunAsync();
