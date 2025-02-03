using UnityEngine;
using TMPro;

public class Stopwatch : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI finalTimer;
    public float timeElapsed = 0f;
    private bool timerRunning = false;

    public void StartTimer()
    {
        timerRunning = true;
    }

    void Update()
    {
        if (timerRunning)
        {
            timeElapsed += Time.deltaTime;
            int minutes = Mathf.FloorToInt(timeElapsed / 60F);
            int seconds = Mathf.FloorToInt(timeElapsed % 60F);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    public void StopTimer()
    {
        timerRunning = false;
    }

    public void ShowLastTimer()
    {
        timeElapsed += Time.deltaTime;
        int minutes = Mathf.FloorToInt(timeElapsed / 60F);
        int seconds = Mathf.FloorToInt(timeElapsed % 60F);
        finalTimer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
