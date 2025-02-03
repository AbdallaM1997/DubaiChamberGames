using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PlaneCollision : MonoBehaviour
{
    public RectTransform plane; // Reference to the plane RectTransform
    public RectTransform canvas; // Reference to the canvas RectTransform
    public Image[] heartImages; // Array of heart images
    public Sprite fullHeartSprite; // Full heart sprite
    public Sprite emptyHeartSprite; // Empty heart sprite
    public Sprite rightSgin;
    public Sprite wrongSgin;
    public GameObject finishPanel; // Finish panel to show when the game ends
    public AudioClip rightSfx;
    public AudioClip wrongSfx;
    public Vector3 offSet = new Vector3(10, 0, 0);
    public int score = 0; // Player score
    public int lives = 3; // Number of lives
    public TextMeshProUGUI timerText; // Stopwatch display
    public TextMeshProUGUI answerText;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI finalTimeText;
    public float elapsedTime = 0f; // Time elapsed for stopwatch

    private DataBaseManager dataBaseManager;
    private CloudSpawner cloudSpawner;
    private bool isCollide = false;
    private bool isDone = false;
    private bool isGameStarted = false;

    private void Start()
    {
        dataBaseManager = GetComponent<DataBaseManager>();
        cloudSpawner = GetComponent<CloudSpawner>();
        // Call StartStopwatch() when game starts
    }

    public void StartStopwatch()
    {
        isGameStarted = true;
        elapsedTime = 0f;
    }

    private void Update()
    {
        if (isGameStarted)
        {
            // Update the stopwatch
            elapsedTime += Time.deltaTime;
            float minutes = Mathf.FloorToInt(elapsedTime / 60);
            float seconds = Mathf.FloorToInt(elapsedTime % 60);
            timerText.text = $"{minutes:00}:{seconds:00}";

            // Detect collision with each cloud in the canvas
            foreach (Transform child in canvas)
            {
                RectTransform cloudTransform = child.GetComponent<RectTransform>();
                if (cloudTransform != null && RectOverlaps(plane, cloudTransform))
                {
                    if (!isCollide)
                        OnTriggerCloud(cloudTransform);
                }
            }

            // If lives reach zero, end the game
            if (lives <= 0 && !isDone)
            {
                EndGame();
            }
        }
    }

    private void OnTriggerCloud(RectTransform cloudTransform)
    {
        // Check if the cloud has the Cloud script attached
        CloudMover cloud = cloudTransform.GetComponent<CloudMover>();
        isCollide = true;
        if (cloud != null)
        {
            cloud.isMoveing = false;
            if (cloud.isCorrectAnswer)
            {
                Debug.Log("Correct answer!");
                AudioSource.PlayClipAtPoint(rightSfx, new Vector3(0, 0, -10f));
                cloudTransform.gameObject.transform.GetChild(1).gameObject.SetActive(true);
                cloudTransform.gameObject.transform.GetChild(1).GetComponent<Image>().sprite = rightSgin;
                cloudTransform.gameObject.transform.GetChild(1).GetComponent<Image>().SetNativeSize();
                answerText.text = cloudTransform.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
                answerText.color = new Color(63f / 255f, 168f / 255f, 42f / 255f, 1);
                score++; // Increment score
                lives--; // Decrement lives
                UpdateLivesDisplay();
            }
            else
            {
                Debug.Log("Wrong answer!");
                AudioSource.PlayClipAtPoint(wrongSfx, new Vector3(0, 0, -10f));
                cloudTransform.gameObject.transform.GetChild(1).gameObject.SetActive(true);
                cloudTransform.gameObject.transform.GetChild(1).GetComponent<Image>().sprite = wrongSgin;
                cloudTransform.gameObject.transform.GetChild(1).GetComponent<Image>().SetNativeSize();
                answerText.text = cloudTransform.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
                answerText.color = Color.red;
                lives--; // Decrement lives
                UpdateLivesDisplay();
            }

            StartCoroutine(DestroyCloudRoutine(cloudTransform));
        }
    }

    private IEnumerator DestroyCloudRoutine(RectTransform cloudTransform)
    {
        yield return new WaitForSeconds(1f);
        if (cloudTransform != null)
            Destroy(cloudTransform.gameObject);
        answerText.color = Color.black;
        answerText.text = "---------";
        isCollide = false;
    }

    private void UpdateLivesDisplay()
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (i < lives)
            {
                heartImages[i].sprite = fullHeartSprite; // Set to full heart
            }
            else
            {
                heartImages[i].sprite = emptyHeartSprite; // Set to empty heart
            }
        }
    }

    private bool RectOverlaps(RectTransform a, RectTransform b)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(a, b.position - offSet, null);
    }

    private void EndGame()
    {
        isDone = true;
        isGameStarted = false;
        cloudSpawner.StopSpawning();
        cloudSpawner.ClearClouds();
        finalScoreText.text = score.ToString();

        float minutes = Mathf.FloorToInt(elapsedTime / 60);
        float seconds = Mathf.FloorToInt(elapsedTime % 60);
        finalTimeText.text = $"{minutes:00}:{seconds:00}";

        dataBaseManager.SendPostRequest();

        // Show the finish panel
        finishPanel.SetActive(true);
    }

    public void PlayAgain(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void StopAudio()
    {
        AudioListener.pause = true;
    }

    public void PlayAudio()
    {
        AudioListener.pause = false;
    }
}
