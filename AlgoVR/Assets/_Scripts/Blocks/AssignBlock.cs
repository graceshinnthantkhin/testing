using UnityEngine;

public class AssignBlock : BlockBase
{
    [Header("Assignment Settings")]
    public string variableName = "x";
    public string value = "5";     // can be interpreted as string or int later

    public override void Execute()
    {
        // Store the variable in the global DataStore (defined below)
        DataStore.SetValue(variableName, value);

        Debug.Log($"?? Assign: {variableName} = {value}");
        OnExecute?.Invoke();
    }

    public override string GetPseudocode()
    {
        // Wrap string values in quotes; numeric values are left as-is
        string formattedValue = int.TryParse(value, out _) ? value : $"\"{value}\"";
        return $"{variableName} = {formattedValue}";
    }
}