using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform letterParent;     // Parent transform where scrambled letters appear
    public Transform slotsParent;      // Parent transform where slots appear
    public TextMeshProUGUI timerText;  // For displaying time left
    public GameObject messageText;     // For displaying success/failure messages
    // If you have a score display, add:
    // public TextMeshProUGUI scoreText;

    [Header("Prefabs")]
    public GameObject letterTilePrefab; // Prefab for letters
    public GameObject slotPrefab;       // Prefab for slots

    [Header("Game Settings")]
    public string solutionWord = "RESPONSIBILITY";
    public float startTime = 30f;

    private int score;
    private float timeRemaining;
    private bool gameActive;

    private List<GameObject> spawnedLetters = new List<GameObject>();
    private List<GameObject> spawnedSlots = new List<GameObject>();

    private DataBaseManager dataBaseManager;

    public int Score
    {
        get => score;
        set
        {
            score = value;
            // If you have a score text UI, update it here:
            // scoreText.text = "Score: " + score;
        }
    }

    private void Start()
    {
        dataBaseManager = GetComponent<DataBaseManager>();
        //SetupGame();
        Score = 0; // Initialize score
    }

    private void Update()
    {
        if (gameActive)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                gameActive = false;
                EndGame(false);
            }

            // Formatting time as "00:00"
            int seconds = Mathf.FloorToInt(timeRemaining);
            timerText.text = seconds.ToString("00") + ":00";
        }
    }

    public void SetupGame()
    {
        // Clear old letters/slots if any
        foreach (var l in spawnedLetters) Destroy(l);
        foreach (var s in spawnedSlots) Destroy(s);
        spawnedLetters.Clear();
        spawnedSlots.Clear();

        // Scramble letters of the solution
        char[] letters = solutionWord.ToCharArray();
        System.Random rnd = new System.Random();
        for (int i = letters.Length - 1; i > 0; i--)
        {
            int j = rnd.Next(0, i + 1);
            char temp = letters[i];
            letters[i] = letters[j];
            letters[j] = temp;
        }

        // Create letters
        foreach (var letter in letters)
        {
            GameObject letterGO = Instantiate(letterTilePrefab, letterParent);
            TextMeshProUGUI letterText = letterGO.GetComponentInChildren<TextMeshProUGUI>();
            letterText.text = letter.ToString();
            spawnedLetters.Add(letterGO);
        }

        // Create slots equal to solution length
        foreach (var c in solutionWord)
        {
            GameObject slotGO = Instantiate(slotPrefab, slotsParent);
            DropSlot slot = slotGO.GetComponent<DropSlot>();
            slot.SetCorrectLetter(c);
            spawnedSlots.Add(slotGO);
        }

        // Wait until letters are spawned before disabling layout
        StartCoroutine(WaitUniltAllLetterShows(letters));

        // Setup timer and start game
        timeRemaining = startTime;
        gameActive = true;
    }

    IEnumerator WaitUniltAllLetterShows(char[] letters)
    {
        yield return new WaitUntil(() => spawnedLetters.Count == letters.Length);
        letterParent.GetComponent<GridLayoutGroup>().enabled = false;
    }

    // This method can still be used to check the final solution if needed
    public void CheckSolution()
    {
        // Check if all slots are filled
        string assembled = "";
        foreach (var slotObj in spawnedSlots)
        {
            DropSlot slot = slotObj.GetComponent<DropSlot>();
            if (slot.transform.childCount == 0)
            {
                // Not all slots filled, no final check
                return;
            }
            // Get the letter placed
            Transform placedLetter = slot.transform.GetChild(0);
            TextMeshProUGUI letterText = placedLetter.GetComponentInChildren<TextMeshProUGUI>();
            assembled += letterText.text;
        }

        // If at the end they form the correct word, end the game
        if (assembled == solutionWord)
        {
            EndGame(true);
        }
    }

    private void EndGame(bool success)
    {
        gameActive = false;
        dataBaseManager.SendPostRequest();
        // Show message text
        messageText.SetActive(true);
        // You can also show final score or success/fail message here
    }

    private void LateUpdate()
    {
        if (gameActive)
        {
            CheckSolution();
        }
    }
}
