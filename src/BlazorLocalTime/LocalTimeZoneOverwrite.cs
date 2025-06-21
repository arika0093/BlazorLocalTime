#pragma warning disable S3011
using System.Reflection;

namespace BlazorLocalTime;

/// <summary>
/// timezone overwriter for test purpose.
/// </summary>
/// <remarks>
/// Windows does not allow to change the value of TimeZoneInfo.Local.
/// so this class overwrites TimeZoneInfo.Local by reflection.
/// this is so dangerous that it should not be used in production.
///
/// refs:
///   https://github.com/dotnet/runtime/discussions/42153
///   https://wandbox.org/permlink/t2NhM89VmelhaFzx
/// </remarks>
public static class LocalTimeZoneOverwrite
{
    /// <summary>
    /// set TimeZoneInfo.Local to UTC.
    /// </summary>
    public static void UseUtc()
    {
        var utc = TimeZoneInfo.Utc;
        SetLocalTimeZoneInfo(
            TimeZoneInfo.CreateCustomTimeZone(
                "UTC-FOR-TEST",
                utc.BaseUtcOffset,
                $"{utc.DisplayName}",
                $"{utc.StandardName}"
            )
        );
    }

    /// <summary>
    /// set TimeZoneInfo.Local to a custom offset.
    /// </summary>
    /// <param name="offset">The custom offset to set.</param>
    public static void UseCustomOffset(TimeSpan offset)
    {
        var sign = offset.Hours switch
        {
            > 0 => "+",
            < 0 => "-",
            _ => string.Empty,
        };
        var customDisplayName = $"({sign}{offset.Hours:D2}:{offset.Minutes:D2}) Custom Time Zone";
        SetLocalTimeZoneInfo(
            TimeZoneInfo.CreateCustomTimeZone(
                "CUSTOM-FOR-TEST",
                offset,
                customDisplayName,
                customDisplayName
            )
        );
    }

    /// <summary>
    /// overwrite the value of TimeZoneInfo.Local
    /// </summary>
    private static void SetLocalTimeZoneInfo(TimeZoneInfo local)
    {
        var cachedDataField = typeof(TimeZoneInfo).GetField(
            "s_cachedData",
            BindingFlags.NonPublic | BindingFlags.Static
        );
        if (cachedDataField == null)
        {
            return;
        }

        var cachedData = cachedDataField.GetValue(null);
        if (cachedData == null)
        {
            return;
        }

        var localTimeZone =
            typeof(TimeZoneInfo)
                .GetNestedType("CachedData", BindingFlags.NonPublic)
                ?.GetField("m_localTimeZone", BindingFlags.NonPublic | BindingFlags.Instance)
            ?? typeof(TimeZoneInfo)
                .GetNestedType("CachedData", BindingFlags.NonPublic)
                ?.GetField("_localTimeZone", BindingFlags.NonPublic | BindingFlags.Instance);
        if (localTimeZone == null)
        {
            return;
        }

        localTimeZone.SetValue(cachedData, local);

        // The following code is for Windows, so if it fails that is ok as we may be running on Linux

        var offsetAndRuleType = typeof(TimeZoneInfo).GetNestedType(
            "OffsetAndRule",
            BindingFlags.NonPublic
        );
        var cInfo = offsetAndRuleType?.GetConstructors(
            BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance
        );
        if (cInfo is not { Length: 1 })
            return;

        TimeZoneInfo.AdjustmentRule?[] rules = local.GetAdjustmentRules();
        var year = DateTime.Now.Year;
        var rule = rules
            .OfType<TimeZoneInfo.AdjustmentRule>()
            .FirstOrDefault(r => r.DateStart.Year == year);

        var o = cInfo[0].Invoke([year, local.BaseUtcOffset, rule]);

        var oneYearLocalFromUtc =
            typeof(TimeZoneInfo)
                .GetNestedType("CachedData", BindingFlags.NonPublic)
                ?.GetField("m_oneYearLocalFromUtc", BindingFlags.NonPublic | BindingFlags.Instance)
            ?? typeof(TimeZoneInfo)
                .GetNestedType("CachedData", BindingFlags.NonPublic)
                ?.GetField("_oneYearLocalFromUtc", BindingFlags.NonPublic | BindingFlags.Instance);
        if (oneYearLocalFromUtc != null)
        {
            oneYearLocalFromUtc.SetValue(cachedData, o);
        }
    }
}
