using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using TMPro;
public class SocketTagDetector : MonoBehaviour
{
    private XRSocketInteractor socketInteractor;
    public string correctTag;
    public GameObject green;
    public GameObject red;
    private GameObject snappedObject;
    //
    public TextMeshProUGUI textPseudo;
    void Awake()
    {
        socketInteractor = GetComponent<XRSocketInteractor>();
    }

    void OnEnable()
    {
        socketInteractor.selectEntered.AddListener(OnItemSnapped);
        socketInteractor.selectExited.AddListener(OnItemExit);
    }

    void OnDisable()
    {
        socketInteractor.selectEntered.RemoveListener(OnItemSnapped);

        socketInteractor.selectExited.RemoveListener(OnItemExit);
    }

    private void OnItemSnapped(SelectEnterEventArgs args)
    {
        // Get the interactable object that just entered
        snappedObject = args.interactableObject.transform.gameObject;
        snappedObject.gameObject.transform.SetParent(this.gameObject.transform);

        if (snappedObject.CompareTag(correctTag))
        {
            Debug.Log("Correct item snapped: " + snappedObject.name);
            green.SetActive(true);
            red.SetActive(false);
            textPseudo.text = "Get pseudo block " + correctTag;
        }
        else 
        {
            green.SetActive(false);
            red.SetActive(true);
            textPseudo.text = "";
        }
        //check if the opbject has extras
        if (snappedObject.TryGetComponent<Extras>(out var extras))
        {
            extras.ShowExtras();
        }
    }
    private void OnItemExit(SelectExitEventArgs args)
    {
        green.SetActive(false);
        red.SetActive(false);

        textPseudo.text = "";

        //check if the opbject has extras
        if (snappedObject.TryGetComponent<Extras>(out var extras))
        {
            extras.HideExtras();
        }
    }
}