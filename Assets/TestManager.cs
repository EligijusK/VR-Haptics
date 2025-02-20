using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestManager : MonoBehaviour
{
    private static TestManager _instance;

    [SerializeField] private AudioSource[] audioClips;
    
    [SerializeField] private AudioSource audioOnAwake;

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

    public static TestManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("TestManager instance not found in the scene. " +
                               "Please add a TestManager GameObject.");
            }
            return _instance;
        }
    }
    
    public void StopAllAudio()
    {
        if (audioOnAwake != null && audioOnAwake.isPlaying)
        {
            audioOnAwake.Stop();
            audioClips[10].Stop();
            Console.WriteLine("sustoja");
        }
        
        foreach (AudioSource clip in audioClips)
        {
            if (clip.isPlaying)
            {
                clip.Stop();
                Console.WriteLine("kazkas juda");
            }
        }
    }
    
    public void PlayClip(int index)
    {
        StopAllAudio();
        audioClips[index].Play();
    }

    public void PatientCanOnly()
    {
        PlayClip(0);
        Console.WriteLine("Gucci");
    }

    public void VisitorsCannotVisit()
    {
        PlayClip(1);
    }

    public void HaveYouListened()
    {
        PlayClip(2);
    }

    public void HowAreYouDressed()
    {
        PlayClip(3);
    }

    public void WeRecommendCompleting()
    {
        PlayClip(4);
    }

    public void WeRecommendChanging()
    {
        PlayClip(5);
    }

    public void WhatShoesHaveYouGot()
    {
        PlayClip(6);
    }

    public void ChooseWhereYouWillGo()
    {
        PlayClip(7);
    }

    public void WeRecommendStarting()
    {
        PlayClip(8);
    }

    public void WrongAnswer()
    {
        PlayClip(9);
    }

    public void TestStart()
    {
        PlayClip(10);
    }
}
