using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SocketRayConnector : MonoBehaviour
{
    private XRRayInteractor rayInteractor;

    void Awake()
    {
        // Get the XRRayInteractor component on the same GameObject
        rayInteractor = GetComponent<XRRayInteractor>();
        if (rayInteractor == null)
            Debug.LogError("SocketRayConnector needs an XRRayInteractor on the same GameObject");
    }

    void Update()
    {
        // Check for primary button press (e.g., trigger)
        // Adjust input based on your controller mapping
        if (Input.GetButtonDown("XRI Right Trigger") || Input.GetButtonDown("XRI Left Trigger"))
        {
            TryConnectToSocket();
        }
    }

    void TryConnectToSocket()
    {
        // Try to get hit information from the ray
        if (rayInteractor.TryGetHitInfo(out Vector3 hitPosition, out Vector3 hitNormal, out int positionInLine, out bool isValidTarget))
        {
            // Perform a physics raycast to get the actual hit object
            Ray ray = new Ray(rayInteractor.rayOriginTransform.position, rayInteractor.rayOriginTransform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, rayInteractor.maxRaycastDistance))
            {
                // Check if the hit object has a CustomSocket component
                CustomSocket socket = hit.collider.GetComponentInParent<CustomSocket>();
                if (socket != null)
                {
                    socket.TrySelectCurrentTarget();
                }
            }
        }
    }
}