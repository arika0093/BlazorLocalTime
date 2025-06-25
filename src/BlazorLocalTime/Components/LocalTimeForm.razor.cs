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
        LocalTimeService.LocalTimeZoneChanged += OnLocalTimeZoneChangedDetailed;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        LocalTimeService.LocalTimeZoneChanged -= OnLocalTimeZoneChangedDetailed;
    }

    private async void OnLocalTimeZoneChangedDetailed(object? sender, TimeZoneChangedEventArgs e)
    {
        // When timezone changes, preserve the UI input value by recalculating the UTC Value
        // based on the current local time displayed in the UI
        var currentValue = ValueAsDateTimeOffset;
        if (currentValue.HasValue && ValueChanged.HasDelegate)
        {
            var prevTimeZone = e.PreviousTimeZone ?? LocalTimeService.BrowserTimeZoneInfo;
            var currTimeZone = e.CurrentTimeZone ?? LocalTimeService.BrowserTimeZoneInfo;
            if (prevTimeZone == null || currTimeZone == null)
            {
                // If either previous or current timezone is null, we cannot proceed with the conversion.
                // This can happen if the browser failed time zone detection.
                return;
            }
            if (prevTimeZone.Equals(currTimeZone))
            {
                // If the time zones are the same, no need to change the value.
                return;
            }
            // Convert the current UTC value to the new local time based on the new time zone.
            var currentUtcVal = currentValue.Value.ToUniversalTime();
            var newValue = new DateTimeOffset(
                currentUtcVal.DateTime + prevTimeZone.BaseUtcOffset,
                currTimeZone.BaseUtcOffset
            );
            var newValueAsT = ConvertValueAsT(newValue);
            await ValueChanged.InvokeAsync(newValueAsT);
        }
        await InvokeAsync(StateHasChanged);
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

    private DateTimeOffset? ValueAsDateTimeOffset
    {
        get
        {
            if (Value is null)
            {
                return null;
            }
            if (typeof(T) == typeof(DateTime) || typeof(T) == typeof(DateTime?))
            {
                var dt = (Value as DateTime?)!.Value.ToUniversalTime();
                return new DateTimeOffset(dt, TimeSpan.Zero);
            }
            if (typeof(T) == typeof(DateTimeOffset) || typeof(T) == typeof(DateTimeOffset?))
            {
                return (Value as DateTimeOffset?)!.Value;
            }
            throw new InvalidOperationException(
                $"Unsupported type {typeof(T)}. Only DateTime, DateTimeOffset, and their nullable versions are supported."
            );
        }
    }

    private static T ConvertValueAsT(DateTimeOffset val)
    {
        if (typeof(T) == typeof(DateTimeOffset) || typeof(T) == typeof(DateTimeOffset?))
        {
            return (T)(object)val;
        }
        // If T is DateTime or Nullable<DateTime>, we need to convert the DateTimeOffset to DateTime.
        if (typeof(T) == typeof(DateTime) || typeof(T) == typeof(DateTime?))
        {
            // Convert the DateTimeOffset to UTC DateTime.
            var utcValue = val.UtcDateTime;
            return (T)(object)utcValue;
        }
        throw new InvalidOperationException(
            $"Unsupported type {typeof(T)}. Only DateTime, DateTimeOffset, and their nullable versions are supported."
        );
    }

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
            throw new InvalidOperationException(
                $"Unsupported type {typeof(T)}. Only DateTime, DateTimeOffset, and their nullable versions are supported."
            );
        }
    }

    private async Task ValueChangedHandler(DateTime? newValue)
    {
        if (ValueChanged.HasDelegate)
        {
            // if the new value is null, we can directly invoke the ValueChanged callback with null.
            if (newValue == null)
            {
                await ValueChanged.InvokeAsync(default);
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
            var convertedValue = ConvertValueAsT(newDateTimeOffset);
            await ValueChanged.InvokeAsync(convertedValue);
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
