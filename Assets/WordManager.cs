using TMPro;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class WordManager : MonoBehaviour
{
    public Button[] answerButtons;    // Buttons for the answers
    public TMP_Text scoreText;        // For the score display
    public TMP_Text timerText;        // For the stopwatch display
    public TMP_Text finalTimerText;  // For the final stopwatch display
    public TMP_Text finalScoreText;  // For the final score display
    public Image[] heartImages;      // Array of heart images for lives
    public GameObject gameOverPanel;
    public AudioClip rightSFX;
    public AudioClip wrongSFX;

    public int score = 0;            // Player's score
    private int lives = 3;           // Player's lives
    private bool isGameActive = false; // Game state
    public float stopwatch = 0f;    // Stopwatch timer
    private DataBaseManager dataBaseManager;

    void Start()
    {
        dataBaseManager = GetComponent<DataBaseManager>();
        // StartGame();
    }

    public void StartGame()
    {
        isGameActive = true;
        stopwatch = 0f; // Reset stopwatch
        LoadQuestion();
        UpdateUI();
    }

    void Update()
    {
        if (isGameActive)
        {
            stopwatch += Time.deltaTime; // Increment the stopwatch
            float minutes = Mathf.FloorToInt(stopwatch / 60);
            float seconds = Mathf.FloorToInt(stopwatch % 60);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    void LoadQuestion()
    {
        // Randomize answer button positions
        RandomizeButtonPositions();

        // Ensure buttons are enabled for interaction
        foreach (Button btn in answerButtons)
        {
            btn.interactable = true;
            // Hide feedback images
            Transform feedbackImage = btn.transform.Find("FeedbackImage");
            if (feedbackImage != null)
            {
                feedbackImage.gameObject.SetActive(false);
            }
        }
    }

    public void CorrectAnswer(Button clickedButton)
    {
        AudioSource.PlayClipAtPoint(rightSFX, new Vector3(0, 0, -10f));
        ShowFeedback(clickedButton, true); // Show correct feedback
        score += 10; // Increase score
        UpdateUI();
        DisableAllButtons();
        GameOver();
    }

    public void WrongAnswer(Button clickedButton)
    {
        AudioSource.PlayClipAtPoint(wrongSFX, new Vector3(0, 0, -10f));
        ShowFeedback(clickedButton, false); // Show wrong feedback
        score = 0; // Reset score
        LoseLife();
        UpdateUI();
        DisableAllButtons();
        GameOver();
    }

    void LoseLife()
    {
        lives--;
        if (lives >= 0)
        {
            heartImages[lives].enabled = false; // Hide a heart
        }

        if (lives <= 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        isGameActive = false;
        gameOverPanel.SetActive(true);
        finalScoreText.text = score.ToString();

        // Display final stopwatch time
        float minutes = Mathf.FloorToInt(stopwatch / 60);
        float seconds = Mathf.FloorToInt(stopwatch % 60);
        finalTimerText.text = $"{minutes:00}:{seconds:00}";

        dataBaseManager.SendPostRequest();

        foreach (Button btn in answerButtons)
        {
            btn.interactable = false; // Disable all buttons
        }
    }

    void UpdateUI()
    {
        scoreText.text = score.ToString(); // Update score text
    }

    void DisableAllButtons()
    {
        foreach (Button btn in answerButtons)
        {
            btn.interactable = false; // Disable all buttons
        }
    }

    void RandomizeButtonPositions()
    {
        // Shuffle the buttons' positions
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int randomIndex = Random.Range(0, answerButtons.Length);
            Vector3 tempPosition = answerButtons[i].transform.position;
            answerButtons[i].transform.position = answerButtons[randomIndex].transform.position;
            answerButtons[randomIndex].transform.position = tempPosition;
        }
    }

    void ShowFeedback(Button clickedButton, bool isCorrect)
    {
        // Find the child "FeedbackImage" of the clicked button
        Transform feedbackImage = clickedButton.transform.GetChild(0);
        if (feedbackImage != null)
        {
            feedbackImage.gameObject.SetActive(true); // Show feedback
        }

        StartCoroutine(HideFeedbackAfterDelay(feedbackImage));
    }

    IEnumerator HideFeedbackAfterDelay(Transform feedbackImage)
    {
        yield return new WaitForSeconds(1f); // Wait for 1 second
        if (feedbackImage != null)
        {
            feedbackImage.gameObject.SetActive(false); // Hide the feedback sprite
        }
    }

    IEnumerator LoadNextQuestionWithDelay()
    {
        yield return new WaitForSeconds(1.5f); // Wait for 1.5 seconds to show feedback
        LoadQuestion();
    }

    public void PlayAgine(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}
