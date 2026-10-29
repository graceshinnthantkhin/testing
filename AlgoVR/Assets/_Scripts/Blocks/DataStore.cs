using System.Collections.Generic;
using UnityEngine;

public static class DataStore
{
    private static Dictionary<string, string> variables = new Dictionary<string, string>();

    public static void SetValue(string key, string val)
    {
        variables[key] = val;
    }

    public static string GetValue(string key)
    {
        return variables.ContainsKey(key) ? variables[key] : null;
    }

    public static void Clear()
    {
        variables.Clear();
    }
}