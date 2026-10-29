using TMPro;
using UnityEngine;

public class CodeDisplay : MonoBehaviour
{
    public TextMeshProUGUI codeText;

    void Start()
    {
        // Listen for pseudocode updates
        GraphManager.Instance.OnPseudocodeUpdated.AddListener(UpdateCode);
    }

    private void UpdateCode(string code)
    {
        if (codeText != null)
            codeText.text = code;
    }

    private void OnDestroy()
    {
        if (GraphManager.Instance != null)
            GraphManager.Instance.OnPseudocodeUpdated.RemoveListener(UpdateCode);
    }
}