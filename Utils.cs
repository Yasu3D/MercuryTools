using UAssetAPI.UnrealTypes;

namespace MercuryTools;

public static class Utils
{
    public static FString? InputOrDefault(string? input)
    {
        return string.IsNullOrEmpty(input) ? null : new(input);
    }
}