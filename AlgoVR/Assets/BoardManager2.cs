using UnityEngine;

public class BoardManager2 : MonoBehaviour
{
    [SerializeField] private GameObject[] boards;

    private int currentIndex = 0;

    private void Start()
    {
        // Make sure only the first board is visible when the game starts
        UpdateVisibility();
    }

    /// <summary>Moves to the next board (wraps around to the first after the last).</summary>
    public void NextBoard()
    {
        if (boards.Length == 0) return;
        currentIndex = (currentIndex + 1) % boards.Length;
        UpdateVisibility();
    }

    /// <summary>Moves to the previous board (wraps around to the last if before the first).</summary>
    public void PreviousBoard()
    {
        if (boards.Length == 0) return;
        currentIndex--;
        if (currentIndex < 0) currentIndex = boards.Length - 1;
        UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        for (int i = 0; i < boards.Length; i++)
        {
            if (i != currentIndex)
            {
                boards[i].SetActive(false);
            }
            else
            {
                boards[i].SetActive(true);
            }
        }
    }
}