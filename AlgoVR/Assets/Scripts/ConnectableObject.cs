using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.Events;

public class ConnectableObject : MonoBehaviour
{
    [Header("Connections")]
    public Transform inputSocketTransform;
    public Transform outputOriginTransform;
    public Transform socketAttachPoint;  // assign the AttachPoint from socket

    [Header("Cable Visual")]
    public LineRenderer cableLine;
    public GameObject plugObject;
    public float cableSag = 0.1f;

    [Header("Events")]
    public UnityEvent<ConnectableObject> onConnected;
    public UnityEvent<string> onConnectionFailed;

    private CustomSocket inputSocket;
    private XRGrabInteractable plugGrab;
    private ConnectableObject connectedTo;
    private bool isOutputConnected = false;
    private Transform originalPlugParent;
    private Vector3 originalPlugLocalPos;
    private Quaternion originalPlugLocalRot;

    protected virtual void Awake()
    {
        // Setup socket
        inputSocket = inputSocketTransform.GetComponent<CustomSocket>();
        if (!inputSocket) inputSocket = inputSocketTransform.gameObject.AddComponent<CustomSocket>();
        inputSocket.SetConnectableObject(this);

        // Setup plug
        plugGrab = plugObject.GetComponent<XRGrabInteractable>();
        if (!plugGrab) plugGrab = plugObject.AddComponent<XRGrabInteractable>();
        plugGrab.selectMode = InteractableSelectMode.Single;

        // Store original plug transform for disconnection
        originalPlugParent = plugObject.transform.parent;
        originalPlugLocalPos = plugObject.transform.localPosition;
        originalPlugLocalRot = plugObject.transform.localRotation;

        // Setup line
        if (cableLine == null) cableLine = GetComponent<LineRenderer>();
        cableLine.positionCount = 2;

        // Configure layer mask
        int outputPlugLayer = LayerMask.NameToLayer("OutputPlug");
        if (outputPlugLayer != -1)
            inputSocket.interactionLayers = 1 << outputPlugLayer;
        else
            Debug.LogError("Layer 'OutputPlug' missing");
    }

    public void ConnectOutputTo(ConnectableObject target)
    {
        if (!CanConnectTo(target))
        {
            onConnectionFailed?.Invoke($"Cannot connect to {target.name}");
            return;
        }

        DisconnectOutput();

        connectedTo = target;
        isOutputConnected = true;

        // Snap the plug to the target's socket attach point
        if (target.socketAttachPoint != null)
        {
            plugObject.transform.SetParent(target.socketAttachPoint);
            plugObject.transform.localPosition = Vector3.zero;
            plugObject.transform.localRotation = Quaternion.identity;
            // Disable grabbing while plugged
            plugGrab.enabled = false;
        }

        target.OnInputConnected(this);
        onConnected?.Invoke(target);
    }

    public void DisconnectOutput()
    {
        if (!isOutputConnected) return;

        // Restore plug to original position/parent
        plugObject.transform.SetParent(originalPlugParent);
        plugObject.transform.localPosition = originalPlugLocalPos;
        plugObject.transform.localRotation = originalPlugLocalRot;
        plugGrab.enabled = true;

        connectedTo?.OnInputDisconnected();
        connectedTo = null;
        isOutputConnected = false;
    }

    public virtual void OnInputConnected(ConnectableObject from)
    {
        inputSocket.enabled = false;
    }

    public virtual void OnInputDisconnected()
    {
        inputSocket.enabled = true;
    }

    public virtual bool CanConnectTo(ConnectableObject other)
    {
        if (other == this) return false;
        if (other.inputSocket.hasSelection) return false;
        if (isOutputConnected) return false;
        return true;
    }

    protected virtual void Update()
    {
        UpdateCableVisual();
    }

    void UpdateCableVisual()
    {
        // Safety checks
        if (cableLine == null) return;
        if (outputOriginTransform == null)
        {
            Debug.LogError("outputOriginTransform is not assigned on " + name);
            return;
        }
        if (plugObject == null)
        {
            Debug.LogError("plugObject is not assigned on " + name);
            return;
        }

        Vector3 start = outputOriginTransform.position;
        Vector3 end;

        if (isOutputConnected && connectedTo != null && connectedTo.inputSocketTransform != null)
        {
            end = connectedTo.inputSocketTransform.position;
        }
        else
        {
            end = plugObject.transform.position;
        }

        // Use world space positions
        cableLine.useWorldSpace = true;

        // Simple two-point line
        cableLine.positionCount = 2;
        cableLine.SetPosition(0, start);
        cableLine.SetPosition(1, end);

        // Optional: Add sag with a middle point
        // Uncomment the lines below for a curved cable
        /*
        cableLine.positionCount = 3;
        Vector3 mid = (start + end) / 2f + Vector3.down * cableSag;
        cableLine.SetPosition(0, start);
        cableLine.SetPosition(1, mid);
        cableLine.SetPosition(2, end);
        */
    }
}