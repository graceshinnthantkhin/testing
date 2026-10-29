using UnityEngine;
using UnityEngine.XR;

public class HandDataReader : MonoBehaviour
{
    public enum Handedness { Left, Right } // Enum makes it easy to select in the Inspector

    [Tooltip("Select which hand this component is attached to.")]
    public Handedness handedness;

    /// <summary>
    /// Public property that other scripts can read to get the hand's current rotation.
    /// </summary>
    public Quaternion HandRotation { get; private set; }

    private UnityEngine.XR.InputDevice targetDevice;

    void Start()
    {
        // Find the correct hand device based on the selected Handedness
        targetDevice = InputDevices.GetDeviceAtXRNode(handedness == Handedness.Left ? XRNode.LeftHand : XRNode.RightHand);
        if (!targetDevice.isValid)
            Debug.LogError($"Unable to find {handedness} hand device!", this);
    }

    void Update()
    {
        if (!targetDevice.isValid) return;

        // Continuously try to get the latest rotation from the hand device
        if (targetDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion newRotation))
        {
            HandRotation = newRotation;
        }
    }
}   