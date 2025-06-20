# BlazorLocalTime

[![NuGet Version](https://img.shields.io/nuget/v/BlazorLocalTime?style=flat-square&logo=NuGet&color=%2308C)](https://www.nuget.org/packages/BlazorLocalTime/) ![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/arika0093/BlazorLocalTime/test.yaml?branch=main&label=Testing&style=flat-square) ![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/arika0093/BlazorLocalTime/release.yaml?branch=main&label=Release&style=flat-square) ![GitHub last commit (branch)](https://img.shields.io/github/last-commit/arika0093/BlazorLocalTime?style=flat-square)

`BlazorLocalTime` provides functionality to convert `DateTime` values to the user's local time zone in Blazor applications.

## Demo
[arika0093.github.io/BlazorLocalTime/](https://arika0093.github.io/BlazorLocalTime/)

## Usage

### Installation
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

### Using as a Component

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

For input forms, it is common to display values in local time and save them as UTC.  
You can easily create such forms using the `LocalTimeForm` component:

```razor
<LocalTimeForm @bind-Value="Dt" Context="dtf">
    <InputDate Type="InputDateType.DateTimeLocal"
        Value="dtf.Value" ValueChanged="dtf.ValueChanged"
        ValueExpression="() => dtf.Value" />
</LocalTimeForm>

@code {
    private DateTime Dt { get; set; } = DateTime.UtcNow;
}
```

Input forms also support separate date and time inputs:

```razor
<LocalTimeForm @bind-Value="Dt" Context="dtf">
    <InputDate Type="InputDateType.Date" Value="dtf.Date" ValueExpression="() => dtf.Date" ValueChanged="dtf.DateChanged" />
    <InputDate Type="InputDateType.Time" Value="dtf.Time" ValueExpression="() => dtf.Time" ValueChanged="dtf.TimeChanged" />
</LocalTimeForm>

@code {
    private DateTime Dt { get; set; } = DateTime.UtcNow;
}
```

### Using as a Service

You can also use `ILocalTimeService` to convert values in your code:

> [!WARNING]  
> During the initial rendering (`OnInitialized`), the user's local time zone may not be available yet, so conversion can fail.  
> Please perform conversions in response to `ILocalTimeService.LocalTimeZoneChanged`.

```razor
@inject ILocalTimeService LocalTimeService
@code {
    private void ButtonClicked()
    {
        var localNow = LocalTimeService.ToLocalTime(DateTime.UtcNow);
    }
}
```

## Reference

[This article](https://www.meziantou.net/convert-datetime-to-user-s-time-zone-with-server-side-blazor-time-provider.htm) was used as a major reference.
