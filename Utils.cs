using System;
using UAssetAPI.PropertyTypes.Objects;

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

    public static bool FilterArray(ArrayPropertyData data, string key, string query, StringComparison comparison)
    {
        for (int i = 0; i < data.Value.Length; i++)
        {
            string? item = data.Value[i].ToString();
            if (item == null) continue;
            if (Filter(item, key, query, comparison)) return true;
        }

        return false;
    }
}