using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Book : MonoBehaviour
{
    [Header("Book Info")]
    [SerializeField] private string title = "Untitled";
    [SerializeField] private string author = "Unknown";

    // Public read-only access
    public string Title => title;
    public string Author => author;

    [Header("UI Setup")]
    [SerializeField] private TextMeshProUGUI displayText;

    private void Start()    
    {
        if (displayText == null)
            displayText = GetComponentInChildren<TextMeshProUGUI>(true);
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (displayText != null)
            displayText.text = $"{author} - {title}";
    }
}