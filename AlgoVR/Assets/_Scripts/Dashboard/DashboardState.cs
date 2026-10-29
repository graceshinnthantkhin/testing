using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SocketEntry
{
    public string socketName;
    public GameObject placedObject; // reference to the XRGrabInteractable placed
}

[Serializable]
public class DashboardState
{
    public List<SocketEntry> socketStates = new List<SocketEntry>();
    // Add any other UI state you need to preserve (e.g., label visibility, text values)
}