using UnityEngine;
using UnityEngine.Events;

public abstract class BlockBase : MonoBehaviour
{
    [Header("Block Definition")]
    public string blockName = "Block";
    [TextArea(2, 5)]
    public string description;

    // For a simple linear sequence, we track only one output.
    // we can later change this to a list for branching.
    [HideInInspector] public BlockBase nextBlock;

    // Events to trigger visual/audio feedback when this block is executed.
    public UnityEvent OnExecute;

    /// <summary>
    /// Called when the algorithm traverses this block.
    /// Override to define custom behaviour (e.g., printing to a console).
    /// </summary>
    public abstract void Execute();

    /// <summary>
    /// Returns the pseudocode line(s) representing this block.
    /// </summary>
    public abstract string GetPseudocode();

    // Connect this block to another (called by the socket combiner)
    public void ConnectTo(BlockBase targetBlock)
    {
        nextBlock = targetBlock;
        Debug.Log($"{blockName} connected to {targetBlock.blockName}");
    }

    // Optional: disconnect
    public void Disconnect()
    {
        nextBlock = null;
    }
}