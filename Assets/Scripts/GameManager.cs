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
    public TextMeshProUGUI timerText;      // Timer display
    public GameObject messageText;         // "Win/Lose" panel
    public TextMeshProUGUI finalTimerText;
    public TextMeshProUGUI finalScoreText;

    [Header("Prefabs")]
    public GameObject letterTilePrefab;    // Clickable letter prefab
    public GameObject slotPrefab;          // Slot prefab (contains a Text or icon)

    [Header("Game Settings")]
    public string solutionWord = "RESPONSIBILITY";
    public float startTime = 30f;          // Puzzle time limit
    [Header("AudioClips")]
    public AudioClip rightAnswer;
    public AudioClip wrongAnswer;
    // Scoring
    public int correctLetterPoints = 10;
    public int wrongLetterPoints = -5;

    private int score;
    private float timeRemaining;
    private bool gameActive;

    // For storing spawned letters and slot references
    private List<GameObject> spawnedLetters = new List<GameObject>();
    private List<TextMeshProUGUI> slotTexts = new List<TextMeshProUGUI>();
    private List<Transform> slotTransforms = new List<Transform>(); // For LERP target positions

    private DataBaseManager dataBaseManager;
    private int nextSlotIndex;  // Which slot is the next correct letter?

    public int Score
    {
        get => score;
        set => score = value;
    }

    private void Start()
    {
        dataBaseManager = GetComponent<DataBaseManager>();
        Score = 0;
    }

    private void Update()
    {
        if (!gameActive) return;

        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            EndGame(true);
        }

        // Update timer text
        float minutes = Mathf.FloorToInt(timeRemaining / 60);
        float seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    /// <summary>
    /// Call this to set up the puzzle: scramble letters, create letters & slots, reset timer, etc.
    /// </summary>
    public void SetupGame()
    {
        // Clean up old
        foreach (var letter in spawnedLetters) Destroy(letter);
        spawnedLetters.Clear();

        foreach (Transform child in slotsParent) Destroy(child.gameObject);
        slotTexts.Clear();
        slotTransforms.Clear();

        // Scramble the solution
        char[] letters = solutionWord.ToCharArray();
        System.Random rnd = new System.Random();
        for (int i = letters.Length - 1; i > 0; i--)
        {
            int j = rnd.Next(0, i + 1);
            (letters[i], letters[j]) = (letters[j], letters[i]);
        }

        // Spawn clickable letters
        foreach (char c in letters)
        {
            GameObject letterGO = Instantiate(letterTilePrefab, letterParent);
            TextMeshProUGUI letterText = letterGO.GetComponentInChildren<TextMeshProUGUI>();
            letterText.text = c.ToString();

            ClickableLetter clickable = letterGO.GetComponent<ClickableLetter>();
            clickable.SetupLetter(this, c);

            spawnedLetters.Add(letterGO);
        }

        // Spawn slots
        foreach (char c in solutionWord)
        {
            GameObject slotGO = Instantiate(slotPrefab, slotsParent);

            // Keep track of slot's text field
            TextMeshProUGUI txt = slotGO.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null)
            {
                txt.text = "_"; // or blank
                slotTexts.Add(txt);
            }
            slotTransforms.Add(slotGO.transform);
        }

        // Reset puzzle state
        StartCoroutine(WaitUntilAllLettersShown(letters));
        timeRemaining = startTime;
        nextSlotIndex = 0;
        gameActive = true;
        messageText.SetActive(false);
    }

    // Optionally disable GridLayout once all letters are created
    private IEnumerator WaitUntilAllLettersShown(char[] letters)
    {
        yield return new WaitUntil(() => spawnedLetters.Count == letters.Length);
        letterParent.GetComponent<GridLayoutGroup>().enabled = false;
    }

    /// <summary>
    /// Called by ClickableLetter when a letter is clicked.
    /// If correct, LERP it to the next slot.
    /// If incorrect, subtract points (no movement).
    /// </summary>
    public void OnLetterClicked(ClickableLetter letter)
    {
        if (!gameActive) return;

        char chosenChar = letter.LetterChar;

        // Check correctness
        if (chosenChar == solutionWord[nextSlotIndex])
        {

            AudioSource.PlayClipAtPoint(rightAnswer, new Vector3(0, 0, -10f));
            // Correct letter => LERP to slot
            Score += correctLetterPoints;
            // Start the coroutine that moves the letter to the correct slot
            StartCoroutine(LerpLetterToSlot(letter, nextSlotIndex, 0.5f)); // 0.5f = duration
            nextSlotIndex++;

            // Check if puzzle is complete
            if (nextSlotIndex >= solutionWord.Length)
            {
                EndGame(true);
            }
        }
        else
        {
            StartCoroutine(WrongSginShow(letter.transform.GetChild(1).gameObject));
            AudioSource.PlayClipAtPoint(wrongAnswer, new Vector3(0, 0, -10f));
            // Wrong letter => just subtract points
            Score += wrongLetterPoints;
            // Optionally, play a "wrong" sound or flash the letter
        }
    }

    IEnumerator WrongSginShow(GameObject objectToShow)
    {
        objectToShow.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        objectToShow.SetActive(false);

    }
    /// <summary>
    /// Moves the letter from its current position to the target slot using a LERP over 'duration' seconds.
    /// </summary>
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

        // Snap to final position
        letter.transform.position = endPos;

        // Optionally re-parent the letter to the slot so it stays there
        letter.transform.SetParent(slotTransforms[slotIndex], worldPositionStays: true);

        //// Update slot text to show the correct letter
        //slotTexts[slotIndex].text = letter.LetterChar.ToString();

        // Disable letter to prevent further clicks
        letter.DisableLetter();
    }

    /// <summary>
    /// Called when time runs out or puzzle completed. 
    /// If success is true, we apply time bonus (if desired).
    /// </summary>
    private void EndGame(bool success)
    {
        if (!gameActive) return;
        gameActive = false;

        if (success)
        {
            // Add a time bonus if puzzle was finished early
            float timeBonus = Mathf.FloorToInt(timeRemaining * 10);
            Score += Mathf.RoundToInt(timeBonus);
        }

        // Update final UI
        finalTimerText.text = timerText.text;
        finalScoreText.text = score.ToString();

        // Send to server if needed
        dataBaseManager.SendPostRequest();

        // Show end message
        messageText.SetActive(true);
    }

    public void PlayAgine(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}
