using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class DropSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private char correctLetter; // Assigned from GameManager
    public AudioClip wrongClip;
    public AudioClip rightClip;
    private GameManager gm;

    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
    }

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
            // Get the dropped letter character
            TextMeshProUGUI letterText = droppedLetter.GetComponentInChildren<TextMeshProUGUI>();
            char droppedChar = letterText.text[0];

            if (transform.childCount == 0)
            {
                // Check correctness
                if (droppedChar == correctLetter)
                {
                    // Correct letter
                    droppedLetter.transform.SetParent(transform);
                    droppedLetter.transform.localPosition = Vector3.zero;
                    droppedLetter.gameObject.GetComponent<RectTransform>().sizeDelta = this.GetComponent<RectTransform>().sizeDelta;
                    gm.Score += 10; // Add 10 points for correct letter
                    AudioSource.PlayClipAtPoint(rightClip, new Vector3(0, 0, -10f));
                }
                else
                {
                    // Wrong letter, deduct 5 points and return to original place
                    gm.Score -= 5;
                    AudioSource.PlayClipAtPoint(wrongClip, new Vector3(0, 0, -10f));
                    droppedLetter.ReturnToOriginal();
                }
            }
            else
            {
                // If slot is already occupied, we simply return the letter to original
                // Or handle swapping if you want that feature.
                droppedLetter.ReturnToOriginal();
            }
        }
    }
}
