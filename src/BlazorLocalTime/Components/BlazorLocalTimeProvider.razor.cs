using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace BlazorLocalTime;

/// <summary>
/// Implements a Blazor component that provides the browser's local time zone information.
/// </summary>
public sealed partial class BlazorLocalTimeProvider : ComponentBase
{
    internal const string JsPath =
        "./_content/BlazorLocalTime/Components/BlazorLocalTimeProvider.razor.js";

    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    [Inject]
    private ILocalTimeService LocalTimeService { get; set; } = null!;

    [Inject]
    private ILogger<BlazorLocalTimeProvider> Logger { get; set; } = null!;

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !LocalTimeService.IsTimeZoneInfoAvailable)
        {
            try
            {
                await using var module = await JsRuntime.InvokeAsync<IJSObjectReference>(
                    "import",
                    JsPath
                );
                var timeZoneString = await module.InvokeAsync<string>("getBrowserTimeZone");
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneString);
                LocalTimeService.SetBrowserTimeZoneInfo(timeZone);
                LocalTimeService.IsSuccessLoadBrowserTimeZone = true;
            }
            catch (JSDisconnectedException ex)
            {
                Logger.LogDebug(
                    ex,
                    "JSDisconnectedException occurred while trying to load browser time zone information. "
                        + "This may happen if the Blazor application is disconnected from the JavaScript runtime."
                );
                LocalTimeService.IsSuccessLoadBrowserTimeZone = false;
            }
            catch (JSException ex)
            {
                Logger.LogWarning(
                    ex,
                    "JSException occurred while trying to load browser time zone information. "
                        + "This may happen if the browser does not support the required JavaScript APIs or if the time zone information is not available."
                );
                LocalTimeService.IsSuccessLoadBrowserTimeZone = false;
            }
        }
    }
}
