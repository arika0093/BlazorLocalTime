namespace BlazorLocalTimeTest;

public class MockTimeProvider(DateTimeOffset fakeUtc) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => fakeUtc;
}
