using System;

namespace MercuryTools;

public static class Utils
{
    public static bool Filter(string? input, string key, string query, StringComparison comparison)
    {
        input ??= "";
        
        string strictInput = $"{key}:{input}";
        if (strictInput.StartsWith(query)) return true;
        if (input.Contains(query, comparison)) return true;

        return false;
    }
}