using UnityEngine;

public class PrintBlock : BlockBase
{
    public string messageToPrint = "Hello, VR!";

    public override void Execute()
    {
        Debug.Log($"🖨 Print: {messageToPrint}");
        // You could also show this message in a VR console UI.
        OnExecute?.Invoke();
    }

    public override string GetPseudocode()
    {
        return $"PRINT \"{messageToPrint}\"";
    }
}
