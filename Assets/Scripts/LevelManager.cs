using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    [Header("Screens")]
    public GameObject startScreen;
    public GameObject startScreenTwo;
    public GameObject gameplayScreen;
    public GameObject instructionsScreen;
    public GameObject leaderboardScreen;
    public GameObject prompetText;

    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_InputField nameInputTwo;

    private CloudSpawner cloudSpawner;
    private PlaneCollision planeCollision;
    private GameManager gameManager;
    private DataBaseManager dataBaseManager;
    private WordManager wordManager;
    private WordSearchGrid wordSearchGrid;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GetComponent<GameManager>();
        cloudSpawner = GetComponent<CloudSpawner>();
        planeCollision = GetComponent<PlaneCollision>();
        dataBaseManager = GetComponent<DataBaseManager>();
        wordManager = GetComponent<WordManager>();
        wordSearchGrid = GetComponent<WordSearchGrid>();
    }
    
    public void StartGame()
    {
        startScreen.SetActive(false);
        instructionsScreen.SetActive(false);
        gameplayScreen.SetActive(true);
        if (planeCollision != null)
            planeCollision.StartStopwatch();
        if (cloudSpawner != null)
            cloudSpawner.StartSpawning();
        if (gameManager != null)
            gameManager.SetupGame();
        if(wordManager != null)
            wordManager.StartGame();
        if(wordSearchGrid != null)
            wordSearchGrid.CreateGrid();
    }

    public void CheckNames()
    {
        if (nameInput.text != "" && nameInputTwo.text != "")
        {
            startScreen.SetActive(false);
            if (startScreenTwo != null)
                startScreenTwo.SetActive(true);
            else
                instructionsScreen.SetActive(true);
        }
        else
        {
            prompetText.SetActive(true);
        }
    }
    public void OpenLeaderboard()
    {
        gameplayScreen.SetActive(false);
        leaderboardScreen.SetActive(true);
        dataBaseManager.GetPostRequest();
    }
}
