using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Import TextMeshPro namespace
using UnityEngine.SceneManagement;

public class WordSearchGrid : MonoBehaviour
{
    public GameObject gridCellPrefab;
    public Transform transformArea;
    public GridLayoutGroup gridLayout;
    public int gridSize = 10;
    public string targetWord = "EXCELLENCE";
    public Image wordImage;
    public Sprite rightSprite;
    public GameObject feedbackPanel;
    public AudioClip correctSFX;
    public AudioClip wrongSFX;
    private List<GameObject> gridCells = new List<GameObject>();
    private List<TextMeshProUGUI> selectedLetters = new List<TextMeshProUGUI>();
    private List<Image> selectedLettersImage = new List<Image>();
    private DataBaseManager dataBaseManager;
    private Stopwatch stopwatch;
    public static WordSearchGrid Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        dataBaseManager = GetComponent<DataBaseManager>();
        stopwatch = GetComponent<Stopwatch>();
    }
    public void CreateGrid()
    {
        stopwatch.StartTimer();
        // Create the grid cells
        for (int i = 0; i < gridSize * gridSize; i++)
        {
            GameObject cell = Instantiate(gridCellPrefab, transformArea);
            gridCells.Add(cell);
        }

        // Insert the target word into the grid
        bool placedHorizontally = Random.value > 0.5f; // Randomly decide if the word is placed horizontally or vertically

        if (placedHorizontally)
        {
            int startRow = Random.Range(0, gridSize);
            int startCol = Random.Range(0, gridSize - targetWord.Length + 1); // Ensure the word fits horizontally

            for (int i = 0; i < targetWord.Length; i++)
            {
                int index = startRow * gridSize + (startCol + i);
                TextMeshProUGUI textComponent = gridCells[index].GetComponentInChildren<TextMeshProUGUI>();
                textComponent.text = targetWord[i].ToString();
            }
        }
        else
        {
            int startCol = Random.Range(0, gridSize);
            int startRow = Random.Range(0, gridSize - targetWord.Length + 1); // Ensure the word fits vertically

            for (int i = 0; i < targetWord.Length; i++)
            {
                int index = (startRow + i) * gridSize + startCol;
                TextMeshProUGUI textComponent = gridCells[index].GetComponentInChildren<TextMeshProUGUI>();
                textComponent.text = targetWord[i].ToString();
            }
        }

        // Fill the remaining cells with random letters
        foreach (GameObject cell in gridCells)
        {
            TextMeshProUGUI textComponent = cell.GetComponentInChildren<TextMeshProUGUI>();
            if (string.IsNullOrEmpty(textComponent.text))
            {
                textComponent.text = GetRandomLetter();
            }
        }
    }
    string GetRandomLetter()
    {
        const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        return alphabet[Random.Range(0, alphabet.Length)].ToString();
    }
    public void OnLetterSelected(TextMeshProUGUI letter , Image butonImage)
    {
        if (!selectedLetters.Contains(letter))
        {
            selectedLetters.Add(letter);
            selectedLettersImage.Add(butonImage);
            butonImage.color = Color.yellow;
            CheckWord();
        }
    }
    void CheckWord()
    {
        string currentWord = "";
        foreach (TextMeshProUGUI letter in selectedLetters)
        {
            currentWord += letter.text;
        }

        if (currentWord == targetWord)
        {
            foreach (Image image in selectedLettersImage)
            {
                image.color = new Color32(31, 149, 56, 160);
            }
            Debug.Log("You found the word: " + currentWord);
            wordImage.sprite = rightSprite;
            wordImage.SetNativeSize();
            AudioSource.PlayClipAtPoint(correctSFX, new Vector3(0, 0, -10f));
            StartCoroutine(WaitToGoToDragArea());
            // Highlight the word or take any desired action
        }
        else if (!targetWord.StartsWith(currentWord))
        {
            // Reset if the selected sequence doesn't match the target word's start
            selectedLetters.Clear();
            foreach (Image image in selectedLettersImage)
            {
                image.color = new Color32(224, 72, 38, 160);
                image.gameObject.transform.parent.transform.GetChild(2).gameObject.SetActive(true);
            }
            AudioSource.PlayClipAtPoint(wrongSFX, new Vector3(0, 0, -10f));
            StartCoroutine(ReturnColorRotuine());
            //selectedLettersImage.Clear();
            Debug.Log("Word reset");
        }
    }

    IEnumerator WaitToGoToDragArea()
    {
        yield return new WaitForSeconds(1.2f);
        stopwatch.ShowLastTimer();
        dataBaseManager.SendPostRequest();
        feedbackPanel.SetActive(true);
    }

    IEnumerator ReturnColorRotuine()
    {
        yield return new WaitForSeconds(0.3f);
        foreach (Image image in selectedLettersImage)
        {
            image.color = new Color(1, 1, 1, 0);
            image.gameObject.transform.parent.transform.GetChild(2).gameObject.SetActive(false);

        }
        selectedLettersImage.Clear();
    }

    public void ResetTheWordSearchGame()
    {
        gridCells.Clear();
        selectedLetters.Clear();
        selectedLettersImage.Clear();
        while(transformArea.childCount > 0)
        {
            DestroyImmediate(transformArea.GetChild(0).gameObject);
        }
    }
    public void PlayAgain(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}
