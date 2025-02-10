using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;

    [SerializeField] private AudioSource[] audioClips;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("AudioManager instance not found in the scene. " +
                               "Please add an AudioManager GameObject.");
            }
            return _instance;
        }
    }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void EnterOperationRoom()
    {
        // Plays audioClips[0]
        if (audioClips[0] != null)
            audioClips[0].Play();
    }

    public void ChooseAntiseptic()
    {
        // Plays audioClips[1]
        if (audioClips[1] != null)
            audioClips[1].Play();
    }
    
    public bool IsAudioPlaying(int index)
    {
        if (audioClips[index] == null) return false;
        return audioClips[index].isPlaying;
    }

    public void ChooseTools()
    {
        audioClips[2].Play();
    }

    public void TakeTampon()
    {
        audioClips[3].Play();
    }

    public void DisinfectRoomAndCover()
    {
        audioClips[4].Play();
    }
    
    public void AttachTheCloth()
    {
        audioClips[5].Play();
    }

    public void ChooseLastCloth()
    {
        audioClips[6].Play();
    }

    public void DisinfectRoomAgainAndIncision()
    {
        audioClips[7].Play();
    }
    
}
