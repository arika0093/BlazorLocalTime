---
name: blazor-local-time
description: Use BlazorLocalTime whenever a Blazor application displays, edits, converts, or otherwise handles a user-facing date or time.
---

# BlazorLocalTime

## Required rule

For every user-facing date or time, use BlazorLocalTime. Do not render an instant with `DateTime.ToString()` or `DateTime.Now`, and do not convert browser-local form input with `DateTime.ToUniversalTime()`; both use the server timezone.

Keep persisted and transmitted instants in UTC. Prefer `DateTimeOffset` where possible.

## Components

- Display a value with `LocalTimeText`.
- Render a custom local-time view with `LocalTime`. Specify `OnLoading` or `OnError` when needed.
- Edit a date or time with `LocalTimeForm`; bind inputs to its supplied context. It converts the local browser input back to UTC.
- Render the current browser timezone with `LocalTimeZone`.
- Use `ILocalTimeService` only for non-component conversion. Check `IsTimeZoneInfoAvailable` before converting. If no library component is rendered, place `<BlazorLocalTimeProvider />` in a root component.

For example, to display a value in local time:

```razor
@* Display a value in local time *@
<LocalTimeText Value="@utcValue" Format="yyyy-MM-dd HH:mm:ssK" />

@* Display a value in local time with a custom view *@
<LocalTime Value="@utcValue" Context="dt">
  @dt.ToString("yyyy-MM-dd HH:mm:ss")
</LocalTime>

@* Form input in local time and save as UTC *@
<LocalTimeForm @bind-Value="Dt" Context="dtf">
  <InputDate Type="InputDateType.DateTimeLocal" @bind-Value="dtf.Value" />
</LocalTimeForm>
```

## Details

Read the [usage guide](https://github.com/arika0093/BlazorLocalTime#using-as-a-component) and [API reference](https://github.com/arika0093/BlazorLocalTime/blob/main/docs/API.md) for parameters, loading states, service events, timezone overrides, and testing.

