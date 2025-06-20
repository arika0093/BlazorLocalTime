# Getting Started

## Installation
Install `BlazorLocalTime` from NuGet:

```bash
dotnet add package BlazorLocalTime
```

## Register the Service
Register the service in your `Program.cs`:

```csharp
builder.Services.AddBlazorLocalTimeService();
```

## Add the Provider
Add the following to `Routes.razor` or `MainLayout.razor`:

```razor
@using BlazorLocalTime
<BlazorLocalTimeProvider />
```

## Sample
- [BlazorLocalTimeExample](../../example/BlazorLocalTimeExample)
  - [Program.cs](../../example/BlazorLocalTimeExample/Program.cs)
  - [Routes.razor](../../example/BlazorLocalTimeExample/Components/Routes.razor)
  - [Home.razor](../../example/BlazorLocalTimeExample/Components/Pages/Home.razor)
