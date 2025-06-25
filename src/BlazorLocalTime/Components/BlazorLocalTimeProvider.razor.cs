using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorLocalTime;

/// <summary>
/// Implements a Blazor component that provides the browser's local time zone information.
/// </summary>
public sealed partial class BlazorLocalTimeProvider : ComponentBase
{
    private const string JsPath =
        "./_content/BlazorLocalTime/Components/BlazorLocalTimeProvider.razor.js";

    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    [Inject]
    private ILocalTimeService LocalTimeService { get; set; } = null!;

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
            }
            catch (JSDisconnectedException)
            {
                // ignore this exception, it means the JS runtime is not available
            }
        }
    }
}
