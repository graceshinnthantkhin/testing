using UnityEngine;

public class IfBlock : BlockBase
{
    [Header("Condition")]
    public string variableName = "x";
    public enum Operator { GreaterThan, LessThan, Equals }
    public Operator op = Operator.GreaterThan;
    public string compareValue = "3";

    [HideInInspector] public BlockBase nextTrueBlock;
    [HideInInspector] public BlockBase nextFalseBlock;

    public override void Execute()
    {
        // Read variable
        string varValueStr = DataStore.GetValue(variableName);
        if (varValueStr == null)
        {
            Debug.LogWarning($"IfBlock: variable '{variableName}' not set. Treating as false.");
            OnExecute?.Invoke();
            return;
        }

        // Compare - we'll do simple numeric comparison for now
        bool conditionMet = EvaluateCondition(varValueStr);

        Debug.Log($"🔀 If ({variableName} {GetOperatorSymbol()} {compareValue})? → {conditionMet}");

        // The GraphManager will check the condition and decide which branch to follow.
        // We expose the result via a property.
        lastConditionResult = conditionMet;
        OnExecute?.Invoke();
    }

    // Helper to check condition
    private bool EvaluateCondition(string actualValueStr)
    {
        if (float.TryParse(actualValueStr, out float actual) && float.TryParse(compareValue, out float compare))
        {
            return op switch
            {
                Operator.GreaterThan => actual > compare,
                Operator.LessThan => actual < compare,
                Operator.Equals => Mathf.Approximately(actual, compare),
                _ => false
            };
        }
        // If not numeric, fall back to string compare
        return string.Compare(actualValueStr, compareValue) == 0;
    }

    public string GetOperatorSymbol() => op switch
    {
        Operator.GreaterThan => ">",
        Operator.LessThan => "<",
        Operator.Equals => "==",
        _ => "?"
    };

    // Store the result for the GraphManager
    [HideInInspector] public bool lastConditionResult;

    // Connection method that socket combiner will call
    public void ConnectToBranch(string branchType, BlockBase targetBlock)
    {
        if (branchType == "true")
            nextTrueBlock = targetBlock;
        else if (branchType == "false")
            nextFalseBlock = targetBlock;
        else
            Debug.LogError($"Unknown branch type '{branchType}'");
    }

    // Pseudocode generation
    public override string GetPseudocode()
    {
        string formattedComp = int.TryParse(compareValue, out _) ? compareValue : $"\"{compareValue}\"";
        return $"IF {variableName} {GetOperatorSymbol()} {formattedComp} THEN ... ELSE ...";
    }
}