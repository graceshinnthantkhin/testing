using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SocketCombiner : MonoBehaviour
{
    private XRSocketInteractor socketInteractor;
    public GameObject parentDevice;   // The main block device (optional)

    void Start()
    {
        socketInteractor = GetComponent<XRSocketInteractor>();

        // --- Prevent the socket from ever scaling plugged objects ---
        socketInteractor.socketScaleMode = SocketScaleMode.None;

        // (Optional) Also disable hover snapping if you want to control attachment manually
        // socketInteractor.hoverSocketSnapping = false;

        socketInteractor.selectEntered.AddListener(OnPluggedIn);
    }

    private void OnPluggedIn(SelectEnterEventArgs args)
    {
        GameObject pluggedObject = args.interactableObject.transform.gameObject;

        // --- Get the original world scale stored on the object ---
        OriginalWorldScale scaleKeeper = pluggedObject.GetComponent<OriginalWorldScale>();
        if (scaleKeeper == null)
        {
            Debug.LogError("Plugged object missing OriginalWorldScale component! Cannot preserve scale.", pluggedObject);
            return;
        }
        Vector3 originalWorldScale = scaleKeeper.originalWorldScale;

        // Determine target parent
        Transform targetParent = parentDevice != null ? parentDevice.transform : transform.parent;
        if (targetParent == null)
        {
            Debug.LogWarning("SocketCombiner: No target parent assigned!");
            return;
        }

        // --- Temporarily detach the object from the socket’s attach point ---
        // This prevents the socket from interfering during the final scale restoration.
        pluggedObject.transform.SetParent(null, true); // move to world space, keep current world pos/rot

        // --- Restore the original world scale while the object is at the root ---
        pluggedObject.transform.localScale = originalWorldScale;

        // --- Now reparent to the desired parent ---
        pluggedObject.transform.SetParent(targetParent, true);

        // After SetParent, the local scale may have been recalculated again
        // because worldScale = localScale * parent.lossyScale. So we force it again.
        Vector3 parentWorldScale = targetParent.lossyScale;
        if (parentWorldScale.x != 0 && parentWorldScale.y != 0 && parentWorldScale.z != 0)
        {
            pluggedObject.transform.localScale = new Vector3(
                originalWorldScale.x / parentWorldScale.x,
                originalWorldScale.y / parentWorldScale.y,
                originalWorldScale.z / parentWorldScale.z
            );
        }
        else
        {
            Debug.LogError("Target parent has zero scale! Cannot compute local scale.");
        }

        // --- Connection logic (unchanged) ---
        CablePlug plugScript = pluggedObject.GetComponent<CablePlug>();
        if (plugScript == null) return;

        BlockBase sourceBlock = plugScript.GetOwnerBlock();
        BlockBase destBlock = GetComponentInParent<BlockBase>();

        if (sourceBlock == null || destBlock == null) return;

        string branchTag = plugScript.GetBranchTag();

        if (sourceBlock is IfBlock ifBlock && !string.IsNullOrEmpty(branchTag))
        {
            ifBlock.ConnectToBranch(branchTag, destBlock);
            Debug.Log($"IfBlock connected ({branchTag}) to {destBlock.blockName}");
        }
        else
        {
            sourceBlock.ConnectTo(destBlock);
        }
    }

    private void OnDestroy()
    {
        if (socketInteractor != null)
            socketInteractor.selectEntered.RemoveListener(OnPluggedIn);
    }
}