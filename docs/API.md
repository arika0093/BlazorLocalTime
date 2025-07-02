# API Reference

## Services

### `ILocalTimeService`
Core service interface for timezone conversion and browser timezone detection.

| Type | Member | Return Type | Description |
|------|--------|-------------|-------------|
| Property | `TimeZoneInfo` | `TimeZoneInfo?` | Current timezone (override or browser-detected) |
| Property | `BrowserTimeZoneInfo` | `TimeZoneInfo?` | Original browser timezone |
| Property | `OverrideTimeZoneInfo` | `TimeZoneInfo?` | User-specified override timezone |
| Property | `Now` | `DateTimeOffset` | Current local time as DateTimeOffset |
| Property | `IsTimeZoneInfoAvailable` | `bool` | Whether timezone info is available |
| Method | `GetBrowserTimeZone()` | `TimeZoneInfo` | Gets browser timezone (throws if unavailable) |
| Method | `ToLocalTime(DateTime utcDateTime)` | `DateTime` | Converts UTC to local DateTime |
| Method | `ToLocalTime(DateTimeOffset dateTimeOffset)` | `DateTime` | Converts DateTimeOffset to local DateTime |
| Method | `ToLocalTimeOffset(DateTime utcDateTime)` | `DateTimeOffset` | Converts UTC to local DateTimeOffset |
| Method | `ToLocalTimeOffset(DateTimeOffset dateTimeOffset)` | `DateTimeOffset` | Converts DateTimeOffset to local DateTimeOffset |
| Event | `LocalTimeZoneChanged` | `Action` | Fired when timezone changes |

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
Provides local time via render fragment with loading and error state support.

| Parameter | Type | Description |
|-----------|------|-------------|
| `Value` | `DateTimeOffset?` | UTC datetime to convert |
| `ChildContent` | `RenderFragment<DateTimeOffset>?` | Content receiving converted time |
| `OnLoading` | `RenderFragment?` | Content displayed while timezone is loading |
| `OnError` | `RenderFragment?` | Content displayed if timezone loading fails |

### `LocalTimeZone`
Provides timezone info via render fragment with loading and error state support.

| Parameter | Type | Description |
|-----------|------|-------------|
| `ChildContent` | `RenderFragment<TimeZoneInfo>?` | Content when timezone available |
| `OnLoading` | `RenderFragment?` | Content displayed while timezone is loading |
| `OnError` | `RenderFragment?` | Content displayed if timezone loading fails |

### `LocalTimeForm<T>`
Form component with timezone conversion for DateTime inputs and loading/error state support.

| Parameter | Type | Description |
|-----------|------|-------------|
| `Value` | `T` | UTC datetime value (DateTime, DateTimeOffset, or nullable versions) |
| `ValueChanged` | `EventCallback<T>` | Value change callback |
| `ChildContent` | `RenderFragment<LocalTimeFormValue>?` | Form content |
| `OnLoading` | `RenderFragment?` | Content displayed while timezone is loading |
| `OnError` | `RenderFragment?` | Content displayed if timezone loading fails |

## Extension Methods

| Method | Description |
|--------|-------------|
| `AddBlazorLocalTimeService()` | Registers BlazorLocalTime services with system TimeProvider |
| `AddBlazorLocalTimeService(TimeProvider timeProvider)` | Registers BlazorLocalTime services with custom TimeProvider |

## Testing Utilities

| Method | Description |
|--------|-------------|
| `LocalTimeZoneOverwrite.UseUtc()` | Force timezone to UTC ⚠️ Test use only |
| `LocalTimeZoneOverwrite.UseCustomOffset(TimeSpan offset)` | Force custom timezone offset ⚠️ Test use only |