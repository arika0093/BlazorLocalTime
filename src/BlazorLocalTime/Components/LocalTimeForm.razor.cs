using Microsoft.AspNetCore.Components;

namespace BlazorLocalTime;

/// <summary>
/// A component that converts a UTC DateTime to the local time of the user and provides it as a render fragment.
/// </summary>
/// <typeparam name="T">T is DateTime, DateTimeOffset, and Nullable versions of these types.</typeparam>
public sealed partial class LocalTimeForm<T> : ComponentBase, IDisposable
{
    [Inject]
    private ILocalTimeService LocalTimeService { get; set; } = null!;

    /// <summary>
    /// A render fragment that receives the local time as a DateTime parameter.
    /// </summary>
    [Parameter]
    public RenderFragment<LocalTimeFormValue>? ChildContent { get; set; }

    /// <summary>
    /// The UTC DateTime value to be converted to local time.
    /// </summary>
    [Parameter]
    public T Value { get; set; } = default!;

    /// <summary>
    /// The format string used to display the local time in the form.
    /// </summary>
    [Parameter]
    public EventCallback<T> ValueChanged { get; set; }

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

    private async Task ValueChangedHandler(DateTime? newValue)
    {
        if (ValueChanged.HasDelegate)
        {
            // if the new value is null, we can directly invoke the ValueChanged callback with null.
            if (newValue == null)
            {
                await ValueChanged.InvokeAsync(default(T));
                return;
            }

            var validVal = newValue.Value;

            // Overwrite the Kind of the DateTime to Unspecified (because AntBlazor DatePicker send as UTC).
            if (validVal.Kind != DateTimeKind.Unspecified)
            {
                validVal = DateTime.SpecifyKind(validVal, DateTimeKind.Unspecified);
            }

            // Since the edited value is in local time, attach the current local time zone information and convert to DateTimeOffset.
            var timeZoneInfo = LocalTimeService.GetBrowserTimeZone();
            var newDateTimeOffset = new DateTimeOffset(
                validVal,
                timeZoneInfo.GetUtcOffset(validVal)
            );
            // if T is DateTimeOffset or Nullable<DateTimeOffset>, we can directly use the newDateTimeOffset.
            if (typeof(T) == typeof(DateTimeOffset) || typeof(T) == typeof(DateTimeOffset?))
            {
                await ValueChanged.InvokeAsync((T)(object)newDateTimeOffset);
                return;
            }
            // If T is DateTime or Nullable<DateTime>, we need to convert the DateTimeOffset to DateTime.
            if (typeof(T) == typeof(DateTime) || typeof(T) == typeof(DateTime?))
            {
                // Convert the DateTimeOffset to UTC DateTime.
                var utcValue = newDateTimeOffset.UtcDateTime;
                await ValueChanged.InvokeAsync((T)(object)utcValue);
                return;
            }
            // If T is neither DateTime nor DateTimeOffset, throw an exception.
            throw new InvalidOperationException(
                $"Unsupported type {typeof(T)}. Only DateTime, DateTimeOffset, and their nullable versions are supported."
            );
        }
    }

    private LocalTimeFormValue FormValue =>
        new()
        {
            Value = LocalTimeValue,
            ValueChanged = EventCallback.Factory.Create<DateTime?>(this, ValueChangedHandler),
            DateChanged = EventCallback.Factory.Create<DateOnly?>(this, DateChangedHandler),
            TimeChanged = EventCallback.Factory.Create<TimeOnly?>(this, TimeChangedHandler),
            TimeSpanChanged = EventCallback.Factory.Create<TimeSpan?>(this, TimespanChangedHandler),
        };

    private DateTime? LocalTimeValue
    {
        get
        {
            if (Value is null)
            {
                return null;
            }
            if (typeof(T) == typeof(DateTime) || typeof(T) == typeof(DateTime?))
            {
                return LocalTimeService.ToLocalTime((Value as DateTime?)!.Value);
            }
            if (typeof(T) == typeof(DateTimeOffset) || typeof(T) == typeof(DateTimeOffset?))
            {
                return LocalTimeService
                    .ToLocalTimeOffset((Value as DateTimeOffset?)!.Value)
                    .DateTime;
            }
            else
            {
                throw new InvalidOperationException(
                    $"Unsupported type {typeof(T)}. Only DateTime, DateTimeOffset, and their nullable versions are supported."
                );
            }
        }
    }

    private async Task DateChangedHandler(DateOnly? newDate)
    {
        if (newDate == null)
        {
            await ValueChanged.InvokeAsync(default(T));
            return;
        }

        var lt = LocalTimeValue ?? LocalTimeService.Now.DateTime;
        var newDateTime = new DateTime(
            newDate.Value.Year,
            newDate.Value.Month,
            newDate.Value.Day,
            lt.Hour,
            lt.Minute,
            lt.Second,
            lt.Kind
        );
        await ValueChangedHandler(newDateTime);
    }

    private async Task TimeChangedHandler(TimeOnly? newTime)
    {
        if (newTime == null)
        {
            await ValueChanged.InvokeAsync(default(T));
            return;
        }

        var lt = LocalTimeValue ?? LocalTimeService.Now.DateTime;
        var newDateTime = new DateTime(
            lt.Year,
            lt.Month,
            lt.Day,
            newTime.Value.Hour,
            newTime.Value.Minute,
            newTime.Value.Second,
            lt.Kind
        );

        await ValueChangedHandler(newDateTime);
    }

    private async Task TimespanChangedHandler(TimeSpan? newTimeSpan)
    {
        if (newTimeSpan == null)
        {
            await ValueChanged.InvokeAsync(default(T));
            return;
        }

        var lt = LocalTimeValue ?? LocalTimeService.Now.DateTime;
        var newDateTime = new DateTime(
            lt.Year,
            lt.Month,
            lt.Day,
            newTimeSpan.Value.Hours,
            newTimeSpan.Value.Minutes,
            newTimeSpan.Value.Seconds,
            lt.Kind
        );

        await ValueChangedHandler(newDateTime);
    }
}
