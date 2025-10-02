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
        var publicApi = typeof(ILocalTimeService).Assembly.GeneratePublicApi(
            new()
            {
                // ignore target framework specific attributes
                IncludeAssemblyAttributes = false,
            }
        );
        // dotnet version to the end of the file name
        // e.g. PublicApiCheckTest.net8-0.approved.txt
        publicApi.ShouldMatchApproved(c =>
        {
            c.SubFolder("Approvals");
        });
    }
}
