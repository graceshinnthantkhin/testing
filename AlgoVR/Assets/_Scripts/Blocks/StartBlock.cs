using UnityEngine;
public class StartBlock : BlockBase
{
    public override void Execute()
    {
        Debug.Log($"▶ Starting algorithm at [{blockName}]");
        OnExecute?.Invoke();
    }

    public override string GetPseudocode()
    {
        return "START";
    }
}