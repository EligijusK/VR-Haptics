using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance;
    private float elapsedTime;
    private bool timerRunning = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    private void Start()
    {
        StartTimer();
    }
    private void Update()
    {
        if (timerRunning)
        {
            elapsedTime += Time.deltaTime;
        }
    }
    
    public void StartTimer()
    {
        timerRunning = true;
    }
    
    public void StopTimer()
    {
        timerRunning = false;
    }
    public void ResetTimer()
    {
        elapsedTime = 0f;
    }
    public float GetElapsedTime()
    {
        return elapsedTime;
    }
}
