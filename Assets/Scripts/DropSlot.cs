using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class DropSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private char correctLetter; // Assign this in the inspector or from the GameManager

    public char CorrectLetter { get { return correctLetter; } }

    public void SetCorrectLetter(char letter)
    {
        correctLetter = letter;
    }

    public void OnDrop(PointerEventData eventData)
    {
        // Check if a draggable letter was dropped
        DraggableLetter droppedLetter = eventData.pointerDrag.GetComponent<DraggableLetter>();
        if (droppedLetter != null)
        {
            // If slot is empty (no child), accept the letter
            if (transform.childCount == 0)
            {
                droppedLetter.transform.SetParent(transform);
                droppedLetter.transform.localPosition = Vector3.zero;
            }
            else
            {
                // If slot is already occupied, you could reject or swap
                // For simplicity, just return the letter where it came from
            }
        }
    }
}
