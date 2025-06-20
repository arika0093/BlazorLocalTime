# Using as a Service

You can use `ILocalTimeService` to convert values in your code.

```razor
@inject ILocalTimeService LocalTimeService
@code {
    protected override void OnInitialized()
    {
        var localNow = LocalTimeService.ToLocalTime(DateTime.UtcNow);
    }
}
```
