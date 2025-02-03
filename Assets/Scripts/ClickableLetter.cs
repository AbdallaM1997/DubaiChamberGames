using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ClickableLetter : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI letterText;

    private GameManager gameManager;
    private char letterChar;
    private bool isDisabled = false;

    public char LetterChar => letterChar;

    public void SetupLetter(GameManager gm, char c)
    {
        gameManager = gm;
        letterChar = c;
        if (letterText) letterText.text = c.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isDisabled)
        {
            gameManager.OnLetterClicked(this);
        }
    }

    public void DisableLetter()
    {
        isDisabled = true;
        // Optionally, fade out or change color to show it's "used"
    }
}
