using Microsoft.AspNetCore.Components;

namespace BlazorLocalTime;

/// <summary>
/// Represents a value for a local time form component.
/// </summary>
public record LocalTimeFormValue
{
    // Since users likely won't create it themselves, the constructor is internal.
    internal LocalTimeFormValue(ILocalTimeService localTimeService)
    {
        LocalTimeService = localTimeService;
    }

    /// <summary>
    /// The local time service used to get the current local time.
    /// </summary>
    private ILocalTimeService LocalTimeService { get; }

    // Backing field for Value property.
    private DateTime? _innerValue;

    /// <summary>
    /// The local time value as a <see cref="DateTime"/>.
    /// </summary>
    public required DateTime? Value
    {
        get => _innerValue;
        set
        {
            _innerValue = value;
            ValueChanged.InvokeAsync(value);
        }
    }

    /// <summary>
    /// The date part of the local time value as a <see cref="DateOnly"/>.
    /// </summary>
    public DateOnly? Date
    {
        get => Value.HasValue ? DateOnly.FromDateTime(Value.Value) : null;
        set => DateChanged.InvokeAsync(value);
    }

    /// <summary>
    /// The time part of the local time value as a <see cref="TimeOnly"/>.
    /// </summary>
    public TimeOnly? Time
    {
        get => Value.HasValue ? TimeOnly.FromDateTime(Value.Value) : null;
        set => TimeChanged.InvokeAsync(value);
    }

    /// <summary>
    /// The time span representation of the local time value (for MudBlazor.TimePicker).
    /// </summary>
    public TimeSpan? TimeSpan
    {
        get =>
            Value.HasValue
                ? new TimeSpan(Value.Value.Hour, Value.Value.Minute, Value.Value.Second)
                : null;
        set => TimeSpanChanged.InvokeAsync(value);
    }

    /// <summary>
    /// It is essentially the same as <see cref="Value"/>. but returns the current date and time when the value is null. <br/>
    /// This is useful when binding values to date and time components that do not tolerate null values.
    /// </summary>
    public DateTime ValueOrNow
    {
        get => _innerValue ?? LocalTimeService.Now.DateTime;
        set => Value = value;
    }

    /// <summary>
    /// It is essentially the same as <see cref="Date"/>, but returns today's date when the value is null. <br/>
    /// This is useful when binding values to date components that do not tolerate null values.
    /// </summary>
    public DateOnly DateOrToday
    {
        get => Date ?? DateOnly.FromDateTime(LocalTimeService.Now.DateTime);
        set => Date = value;
    }

    /// <summary>
    /// It is essentially the same as <see cref="Time"/>, but returns current time when the value is null. <br/>
    /// This is useful when binding values to time components that do not tolerate null values.
    /// </summary>
    public TimeOnly TimeOrNow
    {
        get => Time ?? TimeOnly.FromDateTime(LocalTimeService.Now.DateTime);
        set => Time = value;
    }

    /// <summary>
    /// It is essentially the same as <see cref="TimeSpan"/>, but returns current time when the value is null. <br/>
    /// This is useful when binding values to time components that do not tolerate null values.
    /// </summary>
    public TimeSpan TimeSpanOrNow
    {
        get
        {
            if (TimeSpan.HasValue)
            {
                return TimeSpan.Value;
            }
            var now = LocalTimeService.Now;
            return new TimeSpan(now.Hour, now.Minute, now.Second);
        }
        set => TimeSpan = value;
    }

    /// <summary>
    /// An <see cref="EventCallback"/> that is invoked when the value changes.
    /// </summary>
    public required EventCallback<DateTime?> ValueChanged { get; init; }

    /// <summary>
    /// An <see cref="EventCallback"/> that is invoked when the date changes.
    /// </summary>
    public required EventCallback<DateOnly?> DateChanged { get; init; }

    /// <summary>
    /// An <see cref="EventCallback"/> that is invoked when the time changes.
    /// </summary>
    public required EventCallback<TimeOnly?> TimeChanged { get; init; }

    /// <summary>
    /// An <see cref="EventCallback"/> that is invoked when the time span changes.
    /// </summary>
    public required EventCallback<TimeSpan?> TimeSpanChanged { get; init; }
}
