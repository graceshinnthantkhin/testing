using UnityEngine;

public class EndBlock : BlockBase
{
    public override void Execute()
    {
        Debug.Log($"⏹ Algorithm ended at [{blockName}]");
        OnExecute?.Invoke();
    }

    public override string GetPseudocode()
    {
        return "END";
    }
}
