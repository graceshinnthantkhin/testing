using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GraphManager : MonoBehaviour
{
    public static GraphManager Instance { get; private set; }

    [Header("Execution Settings")]
    public float executionStepDelay = 0.5f;  // pause between blocks

    [Header("Data Token Prefab")]
    public GameObject tokenPrefab;           // glowing sphere prefab

    [HideInInspector] public string collectedPseudocode = "";

    public UnityEvent<string> OnPseudocodeUpdated;  // to update UI

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    [ContextMenu("Execute Graph")]
    public void ExecuteGraph()
    {
        StartCoroutine(ExecuteCoroutine());
    }

    private IEnumerator ExecuteCoroutine()
    {
        collectedPseudocode = "";
        DataStore.Clear();   // fresh start

        StartBlock start = FindObjectOfType<StartBlock>();
        if (start == null) { Debug.LogError("No StartBlock!"); yield break; }

        BlockBase current = start;
        List<Transform> path = new List<Transform>();
        path.Add(current.transform);

        while (current != null)
        {
            // Execute
            current.Execute();
            collectedPseudocode += current.GetPseudocode() + "\n";
            OnPseudocodeUpdated?.Invoke(collectedPseudocode);

            // Determine next block
            if (current is IfBlock ifBlock)
            {
                current = ifBlock.lastConditionResult ? ifBlock.nextTrueBlock : ifBlock.nextFalseBlock;
            }
            else
            {
                current = current.nextBlock;
            }

            if (current != null)
                path.Add(current.transform);

            yield return new WaitForSeconds(executionStepDelay);
        }

        if (tokenPrefab != null && path.Count > 1)
            yield return StartCoroutine(AnimateToken(path));
    }

    // Moves a glowing sphere from block to block
    private IEnumerator AnimateToken(List<Transform> path)
    {
        GameObject token = Instantiate(tokenPrefab, path[0].position, Quaternion.identity);
        float speed = 2f;  // adjust as needed

        for (int i = 0; i < path.Count - 1; i++)
        {
            Vector3 startPos = path[i].position;
            Vector3 endPos = path[i + 1].position;
            float journey = 0f;
            while (journey < 1f)
            {
                journey += Time.deltaTime * speed;
                token.transform.position = Vector3.Lerp(startPos, endPos, journey);
                yield return null;
            }
        }
        Destroy(token, 0.5f); // optional: fade out
    }
}