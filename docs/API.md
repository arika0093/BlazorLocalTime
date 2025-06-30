# API Reference

## Services

### `ILocalTimeService`
Core service interface for timezone conversion and browser timezone detection.

| Type | Member | Description |
|------|--------|-------------|
| Property | `TimeZoneInfo? TimeZoneInfo` | Current timezone (override or browser-detected) |
| Property | `TimeZoneInfo? BrowserTimeZoneInfo` | Original browser timezone |
| Property | `TimeZoneInfo? OverrideTimeZoneInfo` | User-specified override timezone |
| Property | `DateTimeOffset Now` | Current local time as DateTimeOffset |
| Property | `bool IsTimeZoneInfoAvailable` | Whether timezone info is available |
| Method | `TimeZoneInfo GetBrowserTimeZone()` | Gets browser timezone (throws if unavailable) |
| Method | `DateTime ToLocalTime(DateTime utcDateTime)` | Converts UTC to local DateTime |
| Method | `DateTime ToLocalTime(DateTimeOffset dateTimeOffset)` | Converts DateTimeOffset to local DateTime |
| Method | `DateTimeOffset ToLocalTimeOffset(DateTime utcDateTime)` | Converts UTC to local DateTimeOffset |
| Method | `DateTimeOffset ToLocalTimeOffset(DateTimeOffset dateTimeOffset)` | Converts DateTimeOffset to local DateTimeOffset |
| Event | `LocalTimeZoneChanged` | Fired when timezone changes |

## Components

### `BlazorLocalTimeProvider`
Root component for timezone detection. Place in Routes.razor or MainLayout.razor.

| Parameter | Type | Description |
|-----------|------|-------------|
| None | - | No parameters required |

### `LocalTimeText`
Displays formatted local time text.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `DateTimeOffset?` | - | UTC datetime to display |
| `Format` | `string` | `"yyyy-MM-dd HH:mm:ss"` | Display format string |
| `DisableTimeElement` | `bool` | `false` | Whether to wrap in `<time>` element |

### `LocalTime`
Provides local time via render fragment.

| Parameter | Type | Description |
|-----------|------|-------------|
| `Value` | `DateTimeOffset?` | UTC datetime to convert |
| `ChildContent` | `RenderFragment<DateTimeOffset>?` | Content receiving converted time |

### `LocalTimeZone`
Provides timezone info via render fragment.

| Parameter | Type | Description |
|-----------|------|-------------|
| `ChildContent` | `RenderFragment<TimeZoneInfo>?` | Content when timezone available |
| `UnknownContent` | `RenderFragment?` | Content when timezone unavailable |

### `LocalTimeForm<T>`
Form component with timezone conversion for DateTime inputs.

| Parameter | Type | Description |
|-----------|------|-------------|
| `Value` | `T` | UTC datetime value (DateTime, DateTimeOffset, or nullable versions) |
| `ValueChanged` | `EventCallback<T>` | Value change callback |
| `ChildContent` | `RenderFragment<LocalTimeFormValue>?` | Form content |

## Extension Methods

| Method | Description |
|--------|-------------|
| `AddBlazorLocalTimeService()` | Registers BlazorLocalTime services with system TimeProvider |
| `AddBlazorLocalTimeService(TimeProvider timeProvider)` | Registers BlazorLocalTime services with custom TimeProvider |

**Usage:**
```csharp
// With system TimeProvider
builder.Services.AddBlazorLocalTimeService();

// With custom TimeProvider
builder.Services.AddBlazorLocalTimeService(customTimeProvider);
```

## Testing Utilities

| Method | Description |
|--------|-------------|
| `LocalTimeZoneOverwrite.UseUtc()` | Force timezone to UTC ⚠️ Test use only |
| `LocalTimeZoneOverwrite.UseCustomOffset(TimeSpan offset)` | Force custom timezone offset ⚠️ Test use only |