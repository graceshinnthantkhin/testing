using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR;
using UnityEngine.UI; // Needed for the Button component

public class InstantiateBlock : MonoBehaviour
{
    [Header("Instantiation Settings")]
    [Tooltip("Drag the Prefab of your block here.")]
    public GameObject blockPrefab;
    public GameObject SecondBlock;
    [Tooltip("The position where the new block will appear.")]
    public Transform spawnPoint;
    public Transform spawnPoint2;
    private GameObject newBlock;
    private XRInteractionManager interactionManager;

    void Start()
    {
        // Find the XRInteractionManager in the scene. It's a singleton.
        interactionManager = FindObjectOfType<XRInteractionManager>();
        if (interactionManager == null)
        {
            Debug.LogError("XRInteractionManager not found in the scene!", this);
        }
    }

    // This method will be called by the button's OnClick event
    public void SpawnBlock(int value)
    {
        if (blockPrefab == null)
        {
            Debug.LogError("Block Prefab is not assigned!", this);
            return;
        }
        if (spawnPoint == null)
        {
            Debug.LogError("Spawn Point is not assigned!", this);
            return;
        }
        if (value == 1)
        {
            // Instantiate the block at the designated spawn point
             newBlock = Instantiate(blockPrefab, spawnPoint.position, spawnPoint.rotation);
        }
        else if(value == 2)
        {
             newBlock = Instantiate(SecondBlock, spawnPoint2.position, spawnPoint2.rotation);
        }
            // Get all XRGrabInteractable components - we'll need to register the new block with the interaction manager
            XRGrabInteractable[] grabInteractables = newBlock.GetComponents<XRGrabInteractable>();
        foreach (var grabInteractable in grabInteractables)
        {
            if (interactionManager != null)
            {
                // This registers the interactable with the system so it can be grabbed
                //interactionManager.RegisterInteractable(grabInteractable);
                interactionManager.RegisterInteractable((IXRInteractable)this);
            }
        }

        // Optional: Add a bit of randomness to the spawn rotation so blocks don't all spawn identically
        // newBlock.transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
        Debug.Log($"Block '{newBlock.name}' spawned at {spawnPoint.position}.");
    }
}