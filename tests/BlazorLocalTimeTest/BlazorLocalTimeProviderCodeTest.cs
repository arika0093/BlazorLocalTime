using BlazorLocalTime;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Shouldly;

namespace BlazorLocalTimeTest;

public class BlazorLocalTimeProviderCodeTest : TestContext
{
    public BlazorLocalTimeProviderCodeTest()
    {
        Services.AddBlazorLocalTimeService();
        Services.AddLogging();
    }

    [Fact]
    public void AddBlazorLocalTimeService_RegistersServicesForAutomaticInitialization()
    {
        var services = new ServiceCollection();

        services.AddBlazorLocalTimeService();
        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        scope.ServiceProvider.GetRequiredService<ILocalTimeService>().ShouldNotBeNull();
    }

    [Fact]
    public void LocalTimeText_InitializesTimeZoneWithoutProvider_OnlyOnce()
    {
        TestInitializer.JavaScriptInitializer(JSInterop);

        RenderComponent<LocalTimeText>(parameters =>
            parameters.Add(component => component.Value, DateTimeOffset.UtcNow)
        );
        RenderComponent<LocalTimeText>(parameters =>
            parameters.Add(component => component.Value, DateTimeOffset.UtcNow)
        );

        Services.GetRequiredService<ILocalTimeService>().GetBrowserTimeZone().Id.ShouldBe("Asia/Tokyo");
        JSInterop.Invocations.Count(invocation => invocation.Identifier == "import").ShouldBe(1);
    }

    [Fact]
    public void BlazorLocalTimeProvider_SetsTimeZoneSuccessfully()
    {
        TestInitializer.JavaScriptInitializer(JSInterop);

        var component = RenderComponent<BlazorLocalTimeProvider>();
        var localTimeService = Services.GetRequiredService<ILocalTimeService>();

        localTimeService.IsTimeZoneInfoAvailable.ShouldBeTrue();
        localTimeService.IsSuccessLoadBrowserTimeZone?.ShouldBeTrue();
        localTimeService.GetBrowserTimeZone().Id.ShouldBe("Asia/Tokyo");
    }

    [Fact]
    public void BlazorLocalTimeProvider_HandlesJSDisconnectedException()
    {
        var module = JSInterop.SetupModule(BlazorLocalTimeProvider.JsPath);
        module
            .Setup<string>("getBrowserTimeZone")
            .SetException(new JSDisconnectedException("Test disconnection"));

        var component = RenderComponent<BlazorLocalTimeProvider>();
        var localTimeService = Services.GetRequiredService<ILocalTimeService>();

        localTimeService.IsTimeZoneInfoAvailable.ShouldBeFalse();
        localTimeService.IsSuccessLoadBrowserTimeZone?.ShouldBeFalse();
    }

    [Fact]
    public void BlazorLocalTimeProvider_HandlesJSException()
    {
        var module = JSInterop.SetupModule(BlazorLocalTimeProvider.JsPath);
        module
            .Setup<string>("getBrowserTimeZone")
            .SetException(new JSException("Browser API not supported"));

        var component = RenderComponent<BlazorLocalTimeProvider>();
        var localTimeService = Services.GetRequiredService<ILocalTimeService>();

        localTimeService.IsTimeZoneInfoAvailable.ShouldBeFalse();
        localTimeService.IsSuccessLoadBrowserTimeZone?.ShouldBeFalse();
    }

    [Fact]
    public void BlazorLocalTimeProvider_InitialStateIsNull()
    {
        // Don't setup JS interop to simulate initial state
        var localTimeService = Services.GetRequiredService<ILocalTimeService>();

        localTimeService.IsSuccessLoadBrowserTimeZone.ShouldBeNull();
        localTimeService.IsTimeZoneInfoAvailable.ShouldBeFalse();
    }
}
