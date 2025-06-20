# Using in Forms

You can easily create forms that display values in local time and save them as UTC.

## Single Input for DateTime
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

## Separate Inputs for Date and Time
```razor
<LocalTimeForm @bind-Value="Dt" Context="dtf">
    <InputDate Type="InputDateType.Date" Value="dtf.Date" ValueExpression="() => dtf.Date" ValueChanged="dtf.DateChanged" />
    <InputDate Type="InputDateType.Time" Value="dtf.Time" ValueExpression="() => dtf.Time" ValueChanged="dtf.TimeChanged" />
</LocalTimeForm>

@code {
    private DateTime Dt { get; set; } = DateTime.UtcNow;
}
```
