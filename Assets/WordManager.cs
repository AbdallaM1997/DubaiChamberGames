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
    public TMP_Text timerText;        // For the timer display
    public TMP_Text finalTimerText;        // For the timer display
    public TMP_Text finalScoreText;        // For the timer display
    public Image[] heartImages;       // Array of heart images for lives
    public float timer = 15f;        // Countdown timer
    public GameObject gameOverPanel;

    public int score = 0;            // Player's score
    private int lives = 3;            // Player's lives
    private bool isGameActive = false; // Game state
    private DataBaseManager dataBaseManager;
    void Start()
    {
        dataBaseManager = GetComponent<DataBaseManager>();
        //StartGame();
    }

    public void StartGame()
    {
        isGameActive = true;
        LoadQuestion();
        UpdateUI();
    }

    void Update()
    {
        if (isGameActive)
        {
            timer -= Time.deltaTime; // Decrease the timer
            float minutes = Mathf.FloorToInt(timer / 60);
            float seconds = Mathf.FloorToInt(timer % 60);
            timerText.text = $"{minutes:00}:{seconds:00}";

            if (timer <= 0) // If timer runs out
            {
                //LoseLife();
                //LoadQuestion();
                GameOver();
            }
        }
    }

    void LoadQuestion()
    {
        //timer = 10f; // Reset timer


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
        ShowFeedback(clickedButton, true); // Show correct feedback
        score += 10; // Increase score
        UpdateUI();
        DisableAllButtons();
        StartCoroutine(LoadNextQuestionWithDelay());
    }

    public void WrongAnswer(Button clickedButton)
    {
        ShowFeedback(clickedButton, false); // Show wrong feedback
        score -= 5; // Decrease score
        LoseLife();
        UpdateUI();
        DisableAllButtons();
        StartCoroutine(LoadNextQuestionWithDelay());
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
        if (timer <= 0)
        {
            timerText.text = "00:00";
            finalTimerText.text = "00:00";
        }
        else
        {
            float minutes = Mathf.FloorToInt(timer / 60);
            float seconds = Mathf.FloorToInt(timer   % 60);
            finalTimerText.text = $"{minutes:00}:{seconds:00}";
        }
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
