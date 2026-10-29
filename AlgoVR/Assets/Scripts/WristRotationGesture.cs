using UnityEngine;
using UnityEngine.Events; // To create a neat event in the inspector

using UnityEngine.InputSystem;

public class WristRotationGesture : MonoBehaviour
{
    [Tooltip("Reference to the 'Menu' button action from XRI LeftHand.")]
    public InputActionReference toggleAction;

    [Tooltip("The GameObject to show/hide.")]
    public GameObject targetObject;

    private void OnEnable()
    {
        if (toggleAction != null)
        {
            toggleAction.action.Enable();
            toggleAction.action.performed += OnButtonPressed;
        }
    }

    private void OnDisable()
    {
        if (toggleAction != null)
        {
            toggleAction.action.performed -= OnButtonPressed;
            toggleAction.action.Disable();
        }
    }

    private void OnButtonPressed(InputAction.CallbackContext context)
    {
        if (targetObject != null)
        {
            targetObject.SetActive(!targetObject.activeSelf);
        }
    }
}