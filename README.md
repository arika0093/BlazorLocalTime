# BlazorLocalTime

[![NuGet Version](https://img.shields.io/nuget/v/BlazorLocalTime?style=flat-square&logo=NuGet&color=0080CC)](https://www.nuget.org/packages/BlazorLocalTime/) ![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/arika0093/BlazorLocalTime/test.yaml?branch=main&label=Test&style=flat-square) ![GitHub last commit (branch)](https://img.shields.io/github/last-commit/arika0093/BlazorLocalTime?style=flat-square)

`BlazorLocalTime` provides functionality to convert `DateTime` values to the user's local time zone in Blazor Server applications.

[Demo Page](https://arika0093.github.io/BlazorLocalTime/)

## What's this?
The following code contains a bug. Can you spot it?

```razor
<p>@DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")</p>
```

The issue is that this displays the **current time on the server, not the user's local time**.
The time is formatted according to the Blazor host server's time zone, which may not match the user's time zone.

This bug occurs when the Blazor host server's time zone ≠ user's time zone. Because of this, it's an easy bug to overlook during development.

A similar issue arises with date/time input fields. In short, you can't determine "which time zone" the entered time refers to.

```razor
<InputDate Type="InputDateType.DateTimeLocal" @bind-Value="dt" />
@code {
    private DateTime dt { get; set; }
    private void SaveToDatabase()
    {
        // Cannot correctly convert to UTC (uses server's time zone)
        var utc = dt?.ToUniversalTime();
    }
}
```

You can use `BlazorLocalTime` to solve these problems.

## Setup
Install `BlazorLocalTime` from [NuGet](https://www.nuget.org/packages/BlazorLocalTime):

```bash
dotnet add package BlazorLocalTime
```

Next, register the service in your `Program.cs`:

```csharp
builder.Services.AddBlazorLocalTimeService();
```

Finally, add the following component to `Routes.razor` (or `MainLayout.razor`, etc.):

```razor
@using BlazorLocalTime
<BlazorLocalTimeProvider />
```

## Using as a Component

To simply display a local time as text, use the `LocalTimeText` component:

```razor
<LocalTimeText Value="@DateTime.UtcNow" Format="yyyy-MM-dd HH:mm:ss" />
```

Alternatively, you can use the `LocalTime` component to receive the converted value in the child content:

```razor
<LocalTime Value="@DateTime.UtcNow" Context="dt">
    @dt.ToString("yyyy-MM-dd HH:mm:ss")
</LocalTime>
```

If you want to use `TimeZoneInfo`, you can use the `LocalTimeZone` component:

```razor
<LocalTimeZone Context="tz">
    <p>Current Time Zone: @tz.DisplayName</p>
</LocalTimeZone>
```

For input forms, it is common to display values in local time and save them as UTC.  
You can easily create such forms using the `LocalTimeForm` component:

```razor
<LocalTimeForm @bind-Value="Dt" Context="dtf">
    <InputDate Type="InputDateType.DateTimeLocal" @bind-Value="dtf.Value" />
</LocalTimeForm>

@code {
    private DateTime Dt { get; set; } = DateTime.UtcNow;
}
```

Input forms also support separate date and time inputs:

```razor
<LocalTimeForm @bind-Value="Dt" Context="dtf">
    <InputDate Type="InputDateType.Date" @bind-Value="dtf.Date" />
    <InputDate Type="InputDateType.Time" @bind-Value="dtf.Time" />
</LocalTimeForm>

@code {
    private DateTime Dt { get; set; } = DateTime.UtcNow;
}
```

and more usage examples can be found in the [Demo](https://arika0093.github.io/BlazorLocalTime/).

## Using as a Service

You can also use `ILocalTimeService` to convert values in your code:

```razor
@inject ILocalTimeService LocalTimeService
@code {
    private void ButtonClicked()
    {
        var localNow = LocalTimeService.ToLocalTime(DateTime.UtcNow);
        // or shorthand
        // var localNow = LocalTimeService.Now;
    }
}
```

During the initial rendering (`OnInitialized`), the user's local time zone may not be available yet, so conversion can fail.
In such cases, you can use `ILocalTimeService.LocalTimeZoneChanged` to wait until the local time zone becomes available.

```razor
@if(LocalTimeService.IsTimeZoneInfoAvailable)
{
    <p>Current Time is @LocalTimeService.Now</p>
}

@implements IDisposable
@inject ILocalTimeService LocalTimeService
@code {
    protected override void OnInitialized()
    {
        LocalTimeService.LocalTimeZoneChanged += OnLocalTimeZoneChanged;
    }

    public void Dispose()
    {
        LocalTimeService.LocalTimeZoneChanged -= OnLocalTimeZoneChanged;
    }

    private void OnLocalTimeZoneChanged(object? sender, EventArgs e)
    {
        StateHasChanged();
    }
}
```

## Overriding Time Zone

You can programmatically override the browser's detected time zone using `OverrideTimeZoneInfo`. This is useful for testing different time zones or allowing users to select their preferred time zone:

```razor
@inject ILocalTimeService LocalTimeService

<select @onchange="OnTimeZoneChanged">
    <option value="">-- Use Browser Time Zone --</option>
    @foreach (var tz in TimeZoneInfo.GetSystemTimeZones())
    {
        <option value="@tz.Id" selected="@(LocalTimeService.TimeZoneInfo?.Id == tz.Id)">
            @tz.DisplayName
        </option>
    }
</select>
<br/>
Current Time: <LocalTimeText Value="@DateTime.UtcNow" Format="yyyy-MM-dd HH:mm:ssK" />

@code {
    private void OnTimeZoneChanged(ChangeEventArgs e)
    {
        var timeZoneId = e.Value?.ToString();
        LocalTimeService.OverrideTimeZoneInfo = !string.IsNullOrEmpty(timeZoneId)
            ? TimeZoneInfo.FindSystemTimeZoneById(timeZoneId)
            : null;
    }
}
```

The `OverrideTimeZoneInfo` property allows you to:
- Override the browser's detected time zone with any system time zone
- Reset to browser time zone by setting it to `null`
- Automatically triggers `LocalTimeZoneChanged` event when changed

## Testing
When testing, it is not practical to manually change the browser and runtime time zones each time.
To address this, a function is provided to forcibly change the runtime time zone (`TimeZoneInfo.Local`).

```csharp
// UTC
LocalTimeZoneOverwrite.UseUtc();
// Custom Offset (e.g., UTC+9)
LocalTimeZoneOverwrite.UseCustomOffset(TimeSpan.FromHours(9));
```

> [!NOTE]
> Since the demo site is running on `WebAssembly`, the time zone of RunTime normally matches the browser's time zone and should not work well.
Therefore, the above function is executed to force the time zone on the runtime side to be fixed to UTC.

> [!WARNING]
> This feature is intended for testing only. It is not recommended to change `TimeZoneInfo.Local` in production applications.


## Reference

[This article](https://www.meziantou.net/convert-datetime-to-user-s-time-zone-with-server-side-blazor-time-provider.htm) was used as a major reference. I would like to express my gratitude for the reference article.
