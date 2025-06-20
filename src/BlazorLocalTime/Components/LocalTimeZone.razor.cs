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
    /// if the local time zone is unknown, this content will be rendered.
    /// </summary>
    [Parameter]
    public RenderFragment? UnknownContent { get; set; }

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
