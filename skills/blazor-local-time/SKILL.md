---
name: blazor-local-time
description: Use BlazorLocalTime whenever a Blazor application displays, edits, converts, or otherwise handles a user-facing date or time. Use when working with the BlazorLocalTime NuGet package, ILocalTimeService, LocalTimeText, LocalTime, LocalTimeForm, LocalTimeZone, or BlazorLocalTimeProvider.
---

# BlazorLocalTime

## Required rule

For every user-facing date or time, use BlazorLocalTime. Do not render an instant with `DateTime.ToString()` or `DateTime.Now`, and do not convert browser-local form input with `DateTime.ToUniversalTime()`; both use the server timezone.

Keep persisted and transmitted instants in UTC. Prefer `DateTimeOffset` where possible.

## Components

- Display a value with `LocalTimeText`:

  ```razor
  <LocalTimeText Value="@utcValue" Format="yyyy-MM-dd HH:mm:ssK" />
  ```

- Render a custom local-time view with `LocalTime`. Specify `OnLoading` or `OnError` when needed.
- Edit a date or time with `LocalTimeForm`; bind inputs to its supplied context. It converts the local browser input back to UTC.
- Render the current browser timezone with `LocalTimeZone`.
- Use `ILocalTimeService` only for non-component conversion. Check `IsTimeZoneInfoAvailable` before converting. If no library component is rendered, place `<BlazorLocalTimeProvider />` in a root component.

## Details

Read the [usage guide](https://github.com/arika0093/BlazorLocalTime#using-as-a-component) and [API reference](https://github.com/arika0093/BlazorLocalTime/blob/main/docs/API.md) for parameters, loading states, service events, timezone overrides, and testing.

