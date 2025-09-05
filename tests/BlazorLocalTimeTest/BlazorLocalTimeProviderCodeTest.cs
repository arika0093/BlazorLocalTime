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
