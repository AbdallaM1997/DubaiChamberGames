using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Screens")]
    public GameObject startScreen;
    public GameObject gameplayScreen;
    public GameObject instructionsScreen;
    public GameObject leaderboardScreen;

    private CloudSpawner cloudSpawner;
    private PlaneCollision planeCollision;
    private GameManager gameManager;
    private DataBaseManager dataBaseManager;
    private WordManager wordManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GetComponent<GameManager>();
        cloudSpawner = GetComponent<CloudSpawner>();
        planeCollision = GetComponent<PlaneCollision>();
        dataBaseManager = GetComponent<DataBaseManager>();
        wordManager = GetComponent<WordManager>();  
    }
    
    public void StartGame()
    {
        startScreen.SetActive(false);
        instructionsScreen.SetActive(false);
        gameplayScreen.SetActive(true);
        if (planeCollision != null)
            planeCollision.StartTimer();
        if (cloudSpawner != null)
            cloudSpawner.Spwan();
        if (gameManager != null)
            gameManager.SetupGame();
        if(wordManager != null)
            wordManager.StartGame();
    }

    public void OpenLeaderboard()
    {
        gameplayScreen.SetActive(false);
        leaderboardScreen.SetActive(true);
        dataBaseManager.GetPostRequest();
    }
}
