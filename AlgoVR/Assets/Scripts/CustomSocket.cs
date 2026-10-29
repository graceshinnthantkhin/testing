using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class CustomSocket : XRSocketInteractor
{
    private ConnectableObject parentObject;
    private IXRHoverInteractable potentialHoverTarget;  // Store as hover interface

    public void SetConnectableObject(ConnectableObject obj)
    {
        parentObject = obj;
    }

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);
        potentialHoverTarget = args.interactableObject;   // Now works – both are IXRHoverInteractable
        // Optional: visual feedback (e.g., highlight socket)
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);
        if (potentialHoverTarget == args.interactableObject)
        {
            potentialHoverTarget = null;
        }
        // Remove visual feedback
    }

    // Disable automatic selection (we will call it manually)
    public override bool CanSelect(IXRSelectInteractable interactable)
    {
        return false;
    }

    // Call this method from a controller button or other input event
    public void TrySelectCurrentTarget()
    {
        if (potentialHoverTarget == null || parentObject == null)
            return;

        // The hovered object might be the plug's GameObject.
        // We need to get its XRGrabInteractable (which implements IXRSelectInteractable)
        IXRSelectInteractable selectable = potentialHoverTarget.transform.GetComponent<XRGrabInteractable>();
        if (selectable == null)
        {
            parentObject.onConnectionFailed?.Invoke("Hovered object is not selectable");
            return;
        }

        // Find the ConnectableObject that owns this plug
        ConnectableObject source = selectable.transform.GetComponentInParent<ConnectableObject>();
        if (source != null && source != parentObject)
        {
            source.ConnectOutputTo(parentObject);
        }
        else
        {
            parentObject.onConnectionFailed?.Invoke("Cannot connect an object to itself");
        }
    }
}