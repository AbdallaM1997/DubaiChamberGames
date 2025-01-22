using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform letterParent;         // Parent for scrambled letters
    public Transform slotsParent;          // Parent for the solution slots
    public TextMeshProUGUI timerText;      // Stopwatch display
    public GameObject messageText;         // "Win/Lose" panel
    public TextMeshProUGUI finalTimerText;
    public TextMeshProUGUI finalScoreText;

    [Header("Prefabs")]
    public GameObject letterTilePrefab;    // Clickable letter prefab
    public GameObject slotPrefab;          // Slot prefab (contains a Text or icon)

    [Header("Game Settings")]
    public string solutionWord = "RESPONSIBILITY";
    public AudioClip rightAnswer;
    public AudioClip wrongAnswer;
    public int correctLetterPoints = 10;
    public int wrongLetterPoints = -5;

    public float elapsedTime;

    private int score;
    private bool gameActive;

    private List<GameObject> spawnedLetters = new List<GameObject>();
    private List<TextMeshProUGUI> slotTexts = new List<TextMeshProUGUI>();
    private List<Transform> slotTransforms = new List<Transform>();

    private DataBaseManager dataBaseManager;
    private int nextSlotIndex;

    public int Score
    {
        get => score;
        set => score = value;
    }

    private void Start()
    {
        dataBaseManager = GetComponent<DataBaseManager>();
        Score = 0;
        elapsedTime = 0f;
    }

    private void Update()
    {
        if (!gameActive) return;

        elapsedTime += Time.deltaTime;

        float minutes = Mathf.FloorToInt(elapsedTime / 60);
        float seconds = Mathf.FloorToInt(elapsedTime % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void SetupGame()
    {
        foreach (var letter in spawnedLetters) Destroy(letter);
        spawnedLetters.Clear();

        foreach (Transform child in slotsParent) Destroy(child.gameObject);
        slotTexts.Clear();
        slotTransforms.Clear();

        char[] letters = solutionWord.ToCharArray();
        System.Random rnd = new System.Random();
        for (int i = letters.Length - 1; i > 0; i--)
        {
            int j = rnd.Next(0, i + 1);
            (letters[i], letters[j]) = (letters[j], letters[i]);
        }

        foreach (char c in letters)
        {
            GameObject letterGO = Instantiate(letterTilePrefab, letterParent);
            TextMeshProUGUI letterText = letterGO.GetComponentInChildren<TextMeshProUGUI>();
            letterText.text = c.ToString();

            ClickableLetter clickable = letterGO.GetComponent<ClickableLetter>();
            clickable.SetupLetter(this, c);

            spawnedLetters.Add(letterGO);
        }

        foreach (char c in solutionWord)
        {
            GameObject slotGO = Instantiate(slotPrefab, slotsParent);
            TextMeshProUGUI txt = slotGO.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null)
            {
                txt.text = "_";
                slotTexts.Add(txt);
            }
            slotTransforms.Add(slotGO.transform);
        }

        StartCoroutine(WaitUntilAllLettersShown(letters));
        nextSlotIndex = 0;
        gameActive = true;
        messageText.SetActive(false);
    }

    private IEnumerator WaitUntilAllLettersShown(char[] letters)
    {
        yield return new WaitUntil(() => spawnedLetters.Count == letters.Length);
        letterParent.GetComponent<GridLayoutGroup>().enabled = false;
    }

    public void OnLetterClicked(ClickableLetter letter)
    {
        if (!gameActive) return;

        char chosenChar = letter.LetterChar;

        if (chosenChar == solutionWord[nextSlotIndex])
        {
            AudioSource.PlayClipAtPoint(rightAnswer, new Vector3(0, 0, -10f));
            Score += correctLetterPoints;
            StartCoroutine(LerpLetterToSlot(letter, nextSlotIndex, 0.5f));
            nextSlotIndex++;

            if (nextSlotIndex >= solutionWord.Length)
            {
                EndGame(true);
            }
        }
        else
        {
            StartCoroutine(WrongSignShow(letter.transform.GetChild(1).gameObject));
            AudioSource.PlayClipAtPoint(wrongAnswer, new Vector3(0, 0, -10f));
            Score += wrongLetterPoints;
        }
    }

    private IEnumerator WrongSignShow(GameObject objectToShow)
    {
        objectToShow.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        objectToShow.SetActive(false);
    }

    private IEnumerator LerpLetterToSlot(ClickableLetter letter, int slotIndex, float duration)
    {
        Vector3 startPos = letter.transform.position;
        Vector3 endPos = slotTransforms[slotIndex].position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            letter.transform.position = Vector3.Lerp(startPos, endPos, t);
            letter.GetComponent<RectTransform>().sizeDelta = Vector3.Lerp(letter.GetComponent<RectTransform>().sizeDelta,
                slotTransforms[slotIndex].GetComponent<RectTransform>().sizeDelta, 0.05f);
            yield return null;
        }

        letter.transform.position = endPos;
        letter.transform.SetParent(slotTransforms[slotIndex], worldPositionStays: true);
        letter.DisableLetter();
    }

    private void EndGame(bool success)
    {
        if (!gameActive) return;
        gameActive = false;

        finalTimerText.text = timerText.text;
        finalScoreText.text = score.ToString();

        dataBaseManager.SendPostRequest();
        messageText.SetActive(true);
    }

    public void PlayAgain(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}
