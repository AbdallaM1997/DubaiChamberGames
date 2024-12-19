using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Screens")]
    public GameObject startScreen;
    public GameObject gameplayScreen;
    public GameObject instructionsScreen;
    public GameObject leaderboardScreen;

    private GameManager gameManager;
    private DataBaseManager dataBaseManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GetComponent<GameManager>();
        dataBaseManager = GetComponent<DataBaseManager>();
    }
    
    public void StartGame()
    {
        startScreen.SetActive(false);
        instructionsScreen.SetActive(false);
        gameplayScreen.SetActive(true);
        gameManager.SetupGame();
    }

    public void OpenLeaderboard()
    {
        gameplayScreen.SetActive(false);
        leaderboardScreen.SetActive(true);
        dataBaseManager.GetPostRequest();
    }
}
