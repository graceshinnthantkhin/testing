using UnityEngine;
using TMPro;

public class Extras : MonoBehaviour
{
    public GameObject extras;
    public Material[] extraMaterials;
    public MeshRenderer meshRenderer;
    public TextMeshProUGUI label;
    public bool swapMaterial;
    public bool swapLabel;
    private void Start()
    {
        HideExtras();
    }
    public void ShowExtras()
    { extras.SetActive(true); }

    public void HideExtras()
    { extras.SetActive(false); }

    public void SetMaterial(int id)
    { 
        meshRenderer.material = extraMaterials[id];
    }
    public void SetLabel(string value)
    {
        label.text = value;
    }
}
