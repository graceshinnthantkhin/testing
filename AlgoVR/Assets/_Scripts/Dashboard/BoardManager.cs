using System.Collections.Generic;
using UnityEngine;

using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BoardManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject dashboardUIPrefab; // The scene DashboardUI reference
    [SerializeField] private Transform boardParent;         // Where DashboardUIs will be parented
    [SerializeField] private List<UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor> allSockets; // All sockets on the board

    [Header("UI Buttons (3D or Canvas)")]
    [SerializeField] private UnityEngine.UI.Button newButton;
    [SerializeField] private UnityEngine.UI.Button previousButton;
    [SerializeField] private UnityEngine.UI.Button nextButton;
    // Optional: 3D button references if you use XR interactable buttons
    [SerializeField] private XRBaseInteractable newButton3D;
    [SerializeField] private XRBaseInteractable previousButton3D;
    [SerializeField] private XRBaseInteractable nextButton3D;

    [Header("Dashboard Container")]
    [SerializeField] private Transform dashboardContainer; // Parent for instantiated dashboards

    private List<DashboardUIInstance> dashboardInstances = new List<DashboardUIInstance>();
    private int currentIndex = -1;

    private void Start()
    {
        // Hide the original scene DashboardUI and use it as a template
        if (dashboardUIPrefab != null)
            dashboardUIPrefab.SetActive(false);

        // Subscribe to button events
        if (newButton != null)
            newButton.onClick.AddListener(OnNewDashboard);
        if (previousButton != null)
            previousButton.onClick.AddListener(OnPrevious);
        if (nextButton != null)
            nextButton.onClick.AddListener(OnNext);

        // Subscribe to 3D button events (via XRI select)
        if (newButton3D != null)
            newButton3D.selectEntered.AddListener(_ => OnNewDashboard());
        if (previousButton3D != null)
            previousButton3D.selectEntered.AddListener(_ => OnPrevious());
        if (nextButton3D != null)
            nextButton3D.selectEntered.AddListener(_ => OnNext());

        // Create the first dashboard automatically
        CreateNewDashboard();
    }

    private void OnDestroy()
    {
        if (newButton != null)
            newButton.onClick.RemoveListener(OnNewDashboard);
        if (previousButton != null)
            previousButton.onClick.RemoveListener(OnPrevious);
        if (nextButton != null)
            nextButton.onClick.RemoveListener(OnNext);

        if (newButton3D != null)
            newButton3D.selectEntered.RemoveAllListeners();
        if (previousButton3D != null)
            previousButton3D.selectEntered.RemoveAllListeners();
        if (nextButton3D != null)
            nextButton3D.selectEntered.RemoveAllListeners();
    }

    /// <summary>
    /// Creates a brand new dashboard, saves current state first.
    /// </summary>
    public void OnNewDashboard()
    {
        // Save current dashboard state before switching
        SaveCurrentState();

        // Create a fresh dashboard
        CreateNewDashboard();

        // Switch to the new one
        ShowDashboard(dashboardInstances.Count - 1);
    }

    public void OnPrevious()
    {
        if (dashboardInstances.Count == 0) return;

        SaveCurrentState();
        int newIndex = Mathf.Max(0, currentIndex - 1);
        ShowDashboard(newIndex);
    }

    public void OnNext()
    {
        if (dashboardInstances.Count == 0) return;

        SaveCurrentState();
        int newIndex = Mathf.Min(dashboardInstances.Count - 1, currentIndex + 1);
        ShowDashboard(newIndex);
    }

    /// <summary>
    /// Instantiates a new DashboardUI from the prefab.
    /// </summary>
    private void CreateNewDashboard()
    {
        GameObject newDashboardGO = Instantiate(dashboardUIPrefab, dashboardContainer);
        newDashboardGO.SetActive(true);

        // Collect socket references in the new dashboard
        UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor[] sockets = newDashboardGO.GetComponentsInChildren<UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor>();

        DashboardUIInstance instance = new DashboardUIInstance
        {
            gameObject = newDashboardGO,
            sockets = new List<UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor>(sockets),
            state = new DashboardState()
        };

        dashboardInstances.Add(instance);
    }

    /// <summary>
    /// Saves the current dashboard's socket placements into its state.
    /// </summary>
    private void SaveCurrentState()
    {
        if (currentIndex < 0 || currentIndex >= dashboardInstances.Count)
            return;

        DashboardUIInstance current = dashboardInstances[currentIndex];
        current.state.socketStates.Clear();

        foreach (UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socket in current.sockets)
        {
            XRBaseInteractable placed = socket.hasSelection ? socket.interactablesSelected[0] as XRBaseInteractable : null;
            current.state.socketStates.Add(new SocketEntry
            {
                socketName = socket.name,
                placedObject = placed != null ? placed.gameObject : null
            });
        }
    }

    /// <summary>
    /// Hides all dashboards except the one at the given index.
    /// Also restores socket placements.
    /// </summary>
    private void ShowDashboard(int index)
    {
        if (index < 0 || index >= dashboardInstances.Count) return;

        // Hide all
        foreach (DashboardUIInstance instance in dashboardInstances)
        {
            if (instance.gameObject != null)
                instance.gameObject.SetActive(false);
        }

        // Show target
        currentIndex = index;
        DashboardUIInstance target = dashboardInstances[currentIndex];
        target.gameObject.SetActive(true);

        // Restore socket placements
        RestoreState(target);

        // Update button interactability
        UpdateButtonStates();
    }

    /// <summary>
    /// Restores the objects into sockets based on saved state.
    /// </summary>
    private void RestoreState(DashboardUIInstance instance)
    {
        foreach (SocketEntry entry in instance.state.socketStates)
        {
            UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socket = instance.sockets.Find(s => s.name == entry.socketName);
            if (socket != null && entry.placedObject != null)
            {
                // Re-place the object into the socket
                // Note: This assumes the object still exists in the scene/pool.
                // You may need a custom object pooling system for this.
                XRBaseInteractable interactable = entry.placedObject.GetComponent<XRBaseInteractable>();
                if (interactable != null)
                {
                    socket.StartManualInteraction(interactable as IXRSelectInteractable);
                }
            }
        }
    }

    private void UpdateButtonStates()
    {
        bool canGoPrevious = currentIndex > 0;
        bool canGoNext = currentIndex < dashboardInstances.Count - 1;

        if (previousButton != null)
            previousButton.interactable = canGoPrevious;
        if (nextButton != null)
            nextButton.interactable = canGoNext;

        if (previousButton3D != null)
            previousButton3D.enabled = canGoPrevious;
        if (nextButton3D != null)
            nextButton3D.enabled = canGoNext;
    }

    [System.Serializable]
    private class DashboardUIInstance
    {
        public GameObject gameObject;
        public List<UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor> sockets;
        public DashboardState state;
    }
}