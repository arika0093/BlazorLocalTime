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
    private ILocalTimeZoneInitializer Initializer { get; set; } = null!;

    [Inject]
    private ILogger<BlazorLocalTimeProvider> Logger { get; set; } = null!;

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        return firstRender ? Initializer.InitializeAsync(JsRuntime, Logger) : Task.CompletedTask;
    }
}
