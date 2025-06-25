using System.Runtime.CompilerServices;
using BlazorLocalTime;
using Bunit;
using Shouldly;

namespace BlazorLocalTimeTest;

public static class TestInitializer
{
    // this method is called by the test framework before any tests are run
    [ModuleInitializer]
    public static void Initialize()
    {
        // local time zone overwrite for test purpose
        LocalTimeZoneOverwrite.UseUtc();
        // check
        TimeZoneInfo.Local.BaseUtcOffset.ShouldBe(TimeSpan.Zero);
    }

    internal static void JavaScriptInitializer(BunitJSInterop jsInterop)
    {
        var module = jsInterop.SetupModule(BlazorLocalTimeProvider.JsPath);
        module.Setup<string>("getBrowserTimeZone").SetResult("Asia/Tokyo");
    }
}
