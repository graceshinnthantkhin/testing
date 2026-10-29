using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BookManager : MonoBehaviour
{
    [Header("Books")]
    [SerializeField] private List<Book> books = new List<Book>();

    [Header("Slots")]
    // Use empty GameObjects in the scene to mark slot positions
    [SerializeField] private List<Transform> slots = new List<Transform>();

    public enum SortBy { Author, Title }
    public enum SortDirection { AZ, ZA }

    /// <summary>
    /// Sorts books alphabetically by author or title (AZ or ZA),
    /// then moves each book to the matching slot position.
    /// The original books list remains unchanged.
    /// </summary>
    /// 

    public void SortAuthorAZ()
    { 
        SortAndPlace(SortBy.Author, SortDirection.AZ);
    }
    public void SortAuthorZA()
    {
        SortAndPlace(SortBy.Author, SortDirection.ZA);
    }
    public void SortTitleAZ()
    {
        SortAndPlace(SortBy.Title, SortDirection.AZ);
    }
    public void SortTitleTitleZA()
    {
        SortAndPlace(SortBy.Author, SortDirection.ZA);
    }
    public void SortAndPlace(SortBy by, SortDirection direction)
    {
        if (books == null || books.Count == 0)
        {
            Debug.LogWarning("BookManager: No books to sort.");
            return;
        }

        // 1) Read all authors/titles and create a separate sorted list
        List<Book> sortedBooks = new List<Book>(books); // copy original

        System.Func<Book, string> keySelector = by == SortBy.Author
            ? book => book.Author
            : book => book.Title;

        // 2) Sort the copy – original list stays intact
        if (direction == SortDirection.AZ)
            sortedBooks = sortedBooks.OrderBy(keySelector).ToList();
        else
            sortedBooks = sortedBooks.OrderByDescending(keySelector).ToList();

        // 3) One by one, move each book to its slot in the sorted order
        for (int i = 0; i < sortedBooks.Count; i++)
        {
            if (i >= slots.Count)
            {
                Debug.LogWarning($"BookManager: Not enough slots. Only {slots.Count} slots for {sortedBooks.Count} books.");
                break;
            }

            if (slots[i] == null)
            {
                Debug.LogWarning($"BookManager: Slot {i} is null. Skipping.");
                continue;
            }

            sortedBooks[i].transform.position = slots[i].position;
        }
    }
}