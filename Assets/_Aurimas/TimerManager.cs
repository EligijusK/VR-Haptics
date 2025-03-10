using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance;

    private float timer;
    private bool isRunning;

    // Reference to the TextMeshPro UI element.
    public TMP_Text timeText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Persist this object across scenes
        }
        else
        {
            Destroy(gameObject); // Avoid duplicates in new scenes
        }
    }

    private void Start()
    {
        // Only start the timer if we're not in the ExitScene.
        if (SceneManager.GetActiveScene().name != "ExitScene")
        {
            StartTimer();
        }
    }

    private void Update()
    {
        if (isRunning)
            timer += Time.deltaTime;
    }

    public void StartTimer()
    {
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public float GetElapsedTime()
    {
        return timer;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // When ExitScene loads, stop the timer and update the UI.
        if (scene.name == "ExitScene")
        {
            StopTimer();
            
            // Attempt to find the TextMeshPro component if it's not already assigned.
            if (timeText == null)
            {
                GameObject timerObj = GameObject.Find("TimerText");
                if (timerObj != null)
                {
                    timeText = timerObj.GetComponent<TMP_Text>();
                }
                else
                {
                    Debug.LogError("TimerText object not found in the ExitScene.");
                }
            }
            
            if (timeText != null)
            {
                timeText.text = "Time: " + timer.ToString("F2") + " seconds";
            }
        }
    }
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
