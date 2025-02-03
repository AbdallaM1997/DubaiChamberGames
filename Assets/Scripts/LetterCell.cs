using UnityEngine;
using TMPro; // Import TextMeshPro namespace
using UnityEngine.UI;

public class LetterCell : MonoBehaviour
{
    public AudioClip clicked;

    private TextMeshProUGUI letterText;

    void Start()
    {
        letterText = GetComponentInChildren<TextMeshProUGUI>();
        GetComponent<Button>().onClick.AddListener(() => OnCellClicked());
    }

    void OnCellClicked()
    {
        if (WordSearchGrid.Instance != null)
        {
            AudioSource.PlayClipAtPoint(clicked, new Vector3(0, 0, -10f));
            WordSearchGrid.Instance.OnLetterSelected(letterText , transform.GetChild(0).GetComponent<Image>());
        }
    }
}
