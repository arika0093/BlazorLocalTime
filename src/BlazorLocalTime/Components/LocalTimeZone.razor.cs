using Microsoft.AspNetCore.Components;

namespace BlazorLocalTime;

/// <summary>
/// A component that converts a UTC DateTime to the local time of the user.
/// </summary>
public sealed partial class LocalTimeZone : ComponentBase, IDisposable
{
    [Inject]
    private ILocalTimeService LocalTimeService { get; set; } = null!;

    /// <summary>
    /// A render fragment that receives the local time as a DateTime parameter.
    /// </summary>
    [Parameter]
    public RenderFragment<TimeZoneInfo>? ChildContent { get; set; }

    /// <summary>
    /// A render fragment that is displayed while the browser's time zone is being loaded.
    /// </summary>
    [Parameter]
    public RenderFragment? OnLoading { get; set; }

    /// <summary>
    /// A render fragment that is displayed if an error occurs while loading the browser's time zone.
    /// </summary>
    [Parameter]
    public RenderFragment? OnError { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        LocalTimeService.LocalTimeZoneChanged += OnLocalTimeZoneChanged;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        LocalTimeService.LocalTimeZoneChanged -= OnLocalTimeZoneChanged;
    }

    private void OnLocalTimeZoneChanged(object? sender, EventArgs e)
    {
        StateHasChanged();
    }
}
