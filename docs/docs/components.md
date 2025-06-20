# Component Usage

## Display Local Time as Text
You can use the `LocalTimeText` component to display local time as text.

```razor
<LocalTimeText Value="@DateTime.UtcNow" Format="yyyy/MM/dd HH:mm:ss" />
```

## Use Local Time Value in Child Content
The `LocalTime` component allows you to use the converted value in the child content.

```razor
<LocalTime Value="@DateTime.UtcNow" Context="dt">
    @dt.ToString("yyyy/MM/dd HH:mm:ss")
</LocalTime>
```
