using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;

    [SerializeField] private AudioSource[] audioClips;
    [SerializeField] private AudioSource[] sfxList;

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

    public void OnlySelectNeededInstruments()
    {
        StopAllAudio();
        if (audioClips[9] != null)
            audioClips[9].Play();
    }

    public void ListNeededInstruments()
    {
        StopAllAudio();
        if (audioClips[10] != null)
            audioClips[10].Play();
    }

    public void UseSyringe()
    {
        StopAllAudio();
        if (audioClips[11] != null)
            audioClips[11].Play();
    }

    public void UseBandage()
    {
        StopAllAudio();
        if (audioClips[12] != null)
            audioClips[12].Play();
    }
    
    //  V V VSFX SOUND FUNCTION CALLS V V V

    public void OpenDoor()
    {
        if (sfxList[6] != null)
            sfxList[6].Play();
    }

    public void HoverTestSelection()
    {
        if (sfxList[7] != null)
            sfxList[7].Play();
    }

    public void TestSelectionCorrect()
    {
        if (sfxList[5] != null)
            sfxList[5].Play();
    }

    public void AntisepticBottleScrew()
    {
        if (sfxList[4] != null)
            sfxList[4].Play();
    }

    public int AntisepticFlow()
    {
        if (sfxList.Length < 4) return 3;

        int randomIndex = Random.Range(0, 4);
    
        if (sfxList[randomIndex] != null)
        {
            sfxList[randomIndex].Play();
        }

        return (randomIndex == 3) ? 5 : 3;
    }

}
