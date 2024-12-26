using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform letterParent;     // Parent transform where scrambled letters appear
    public Transform slotsParent;      // Parent transform where slots appear
    public TextMeshProUGUI timerText;  // For displaying time left
    public GameObject messageText;     // For displaying success/failure messages
    public TextMeshProUGUI finalTimerText;  // For displaying time left
    public TextMeshProUGUI finalScoreText;  // For displaying time left

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
        }
    }

    private void Start()
    {
        dataBaseManager = GetComponent<DataBaseManager>();
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
                EndGame(true);
            }

            float minutes = Mathf.FloorToInt(timeRemaining / 60);
            float seconds = Mathf.FloorToInt(timeRemaining % 60);

            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    public void SetupGame()
    {
        foreach (var l in spawnedLetters) Destroy(l);
        foreach (var s in spawnedSlots) Destroy(s);
        spawnedLetters.Clear();
        spawnedSlots.Clear();

        char[] letters = solutionWord.ToCharArray();
        System.Random rnd = new System.Random();
        for (int i = letters.Length - 1; i > 0; i--)
        {
            int j = rnd.Next(0, i + 1);
            char temp = letters[i];
            letters[i] = letters[j];
            letters[j] = temp;
        }

        foreach (var letter in letters)
        {
            GameObject letterGO = Instantiate(letterTilePrefab, letterParent);
            TextMeshProUGUI letterText = letterGO.GetComponentInChildren<TextMeshProUGUI>();
            letterText.text = letter.ToString();
            spawnedLetters.Add(letterGO);
        }

        foreach (var c in solutionWord)
        {
            GameObject slotGO = Instantiate(slotPrefab, slotsParent);
            DropSlot slot = slotGO.GetComponent<DropSlot>();
            slot.SetCorrectLetter(c);
            spawnedSlots.Add(slotGO);
        }

        StartCoroutine(WaitUniltAllLetterShows(letters));

        timeRemaining = startTime;
        gameActive = true;
    }

    IEnumerator WaitUniltAllLetterShows(char[] letters)
    {
        yield return new WaitUntil(() => spawnedLetters.Count == letters.Length);
        letterParent.GetComponent<GridLayoutGroup>().enabled = false;
    }

    public void CheckSolution()
    {
        string assembled = "";
        foreach (var slotObj in spawnedSlots)
        {
            DropSlot slot = slotObj.GetComponent<DropSlot>();
            if (slot.transform.childCount == 0)
            {
                return;
            }
            Transform placedLetter = slot.transform.GetChild(0);
            TextMeshProUGUI letterText = placedLetter.GetComponentInChildren<TextMeshProUGUI>();
            assembled += letterText.text;
        }

        if (assembled == solutionWord)
        {
            EndGame(true);
        }
    }

    private void EndGame(bool success)
    {
        gameActive = false;

        if (success)
        {
            float timeBonus = Mathf.FloorToInt(timeRemaining * 10); // Time bonus calculation
            Score += Mathf.RoundToInt(timeBonus); // Add time bonus to score
        }
        dataBaseManager.SendPostRequest();
        finalTimerText.text = timerText.text;
        finalScoreText.text = score.ToString();
        messageText.SetActive(true);
    }

    private void LateUpdate()
    {
        if (gameActive)
        {
            CheckSolution();
        }
    }
    public void PlayAgine()
    {
        SceneManager.LoadScene("Game1");
    }
}
