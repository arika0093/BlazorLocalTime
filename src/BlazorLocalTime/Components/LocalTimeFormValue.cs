using Microsoft.AspNetCore.Components;

namespace BlazorLocalTime;

/// <summary>
/// Represents a value for a local time form component.
/// </summary>
public record LocalTimeFormValue
{
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
        get =>
            Value.HasValue
                ? new DateOnly(Value.Value.Year, Value.Value.Month, Value.Value.Day)
                : null;
        set => DateChanged.InvokeAsync(value);
    }

    /// <summary>
    /// The time part of the local time value as a <see cref="TimeOnly"/>.
    /// </summary>
    public TimeOnly? Time
    {
        get =>
            Value.HasValue
                ? new TimeOnly(Value.Value.Hour, Value.Value.Minute, Value.Value.Second)
                : null;
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
