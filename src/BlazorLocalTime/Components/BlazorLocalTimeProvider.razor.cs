using System.Globalization;
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

    [Inject]
    private BlazorLocalTimeConfiguration Configuration { get; set; } = null!;

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !LocalTimeService.IsTimeZoneInfoAvailable)
        {
            TimeZoneInfo? timeZone = null;
            try
            {
                await using var module = await JsRuntime.InvokeAsync<IJSObjectReference>(
                    "import",
                    JsPath
                );
                var timeZoneString = await module.InvokeAsync<string>("getBrowserTimeZone");
                if (!ICUMode())
                {
                    // On Windows with NLS mode, IANA time zone are must be converted to Windows time zone.
                    var converter = Configuration.IanaToWindows;
                    if (converter == null)
                    {
                        var message = """
                            In older Windows environments, IANA time zones (such as “Asia/Tokyo”) cannot be used directly.
                            For details, see https://github.com/arika0093/BlazorLocalTime/issues/19.
                            """;
                        throw new TimeZoneNotFoundException(message);
                    }
                    timeZoneString = converter(timeZoneString);
                }
                timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneString);
            }
            catch (JSDisconnectedException ex)
            {
                Logger.LogDebug(
                    ex,
                    "JSDisconnectedException occurred while trying to load browser time zone information. "
                        + "This may happen if the Blazor application is disconnected from the JavaScript runtime."
                );
            }
            catch (JSException ex)
            {
                Logger.LogWarning(
                    ex,
                    "JSException occurred while trying to load browser time zone information. "
                        + "This may happen if the browser does not support the required JavaScript APIs or if the time zone information is not available."
                );
            }
            finally
            {
                LocalTimeService.IsSuccessLoadBrowserTimeZone = (timeZone != null);
                LocalTimeService.SetBrowserTimeZoneInfo(timeZone);
            }
        }
    }

    // https://learn.microsoft.com/en-us/dotnet/core/extensions/globalization-icu#determine-if-your-app-is-using-icu
    private static bool ICUMode()
    {
        SortVersion sortVersion = CultureInfo.InvariantCulture.CompareInfo.Version;
        byte[] bytes = sortVersion.SortId.ToByteArray();
        int version = bytes[3] << 24 | bytes[2] << 16 | bytes[1] << 8 | bytes[0];
        return version != 0 && version == sortVersion.FullVersion;
    }
}
