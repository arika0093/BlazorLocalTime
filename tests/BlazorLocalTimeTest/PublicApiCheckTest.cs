using System.Runtime.InteropServices;
using BlazorLocalTime;
using PublicApiGenerator;
using Shouldly;

namespace BlazorLocalTimeTest;

public class PublicApiCheckTest
{
    [Fact]
    public void Run()
    {
        var publicApi = typeof(ILocalTimeService).Assembly.GeneratePublicApi();
        // dotnet version to the end of the file name
        // e.g. PublicApiCheckTest.net8-0.approved.txt
        var dotnetVersion = GetDotnetVersion();
        publicApi.ShouldMatchApproved(c =>
        {
            c.SubFolder("Approvals");
            c.WithDiscriminator(dotnetVersion);
        });
    }

    private string GetDotnetVersion()
    {
        var version = Environment.Version;
        var majorVersion = version.Major;
        return $"net{majorVersion}";
    }
}
