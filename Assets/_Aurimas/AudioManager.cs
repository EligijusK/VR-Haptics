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
    
    public void StopAllAudio()
    {
        foreach (var source in audioClips)
        {
            if (source != null && source.isPlaying)
            {
                source.Stop();
            }
        }
    }

    public void EnterOperationRoom()
    {
        StopAllAudio();
        if (audioClips[0] != null)
            audioClips[0].Play();
    }


    public void ChooseAntiseptic()
    {
        StopAllAudio();
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
        StopAllAudio();
        if (audioClips[2] != null)
            audioClips[2].Play();
    }

    public void TakeTampon()
    {
        StopAllAudio();
        if (audioClips[3] != null)
            audioClips[3].Play();
    }

    public void DisinfectRoomAndCover()
    {
        StopAllAudio();
        if (audioClips[4] != null)
            audioClips[4].Play();
    }

    public void AttachTheCloth()
    {
        StopAllAudio();
        if (audioClips[5] != null)
            audioClips[5].Play();
    }

    public void ChooseLastCloth()
    {
        StopAllAudio();
        if (audioClips[6] != null)
            audioClips[6].Play();
    }

    public void DisinfectRoomAgainAndIncision()
    {
        StopAllAudio();
        if (audioClips[7] != null)
            audioClips[7].Play();
    }

    public void FallenInstrument()
    {
        StopAllAudio();
        if (audioClips[8] != null)
            audioClips[8].Play();
    }
    
}
