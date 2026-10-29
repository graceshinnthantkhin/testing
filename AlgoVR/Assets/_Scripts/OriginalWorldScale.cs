// OriginalWorldScale.cs
using UnityEngine;

public class OriginalWorldScale : MonoBehaviour
{
    public Vector3 originalWorldScale { get; private set; }

    void Awake()
    {
        originalWorldScale = transform.lossyScale;
    }
}