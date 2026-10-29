using UnityEngine;

public class CablePlug : MonoBehaviour
{
    public string branchTag = "";   // "true", "false", or empty for normal blocks
    // The block that owns this plug.
    private BlockBase ownerBlock;

    void Awake()
    {
        // Assumes the plug is a child/grandchild of the BlockRoot
        ownerBlock = GetComponentInParent<BlockBase>();
        if (ownerBlock == null)
            Debug.LogError($"CablePlug on {name} could not find a BlockBase in parents!", this);
    }

    public BlockBase GetOwnerBlock()
    {
        return ownerBlock;
    }
    public string GetBranchTag()
    { 
        return branchTag;
    }
}
