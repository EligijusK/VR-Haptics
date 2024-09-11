using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private TrackMainCamera videoMonitor;
    [SerializeField] private OrderingInLine orderingInLine;
    bool stopedTracking = false;
    // Start is called before the first frame update
    void Start()
    {
        videoPlayer.loopPointReached += EndReached;
    }

    void EndReached(VideoPlayer vp)
    {
        // videoMonitor.StopTracking();
        // orderingInLine.StartBlinkingTimer();
        // stopedTracking = true;
    }
    // Update is called once per frame
    void Update()
    {
        // if ( !videoPlayer.isPlaying && !stopedTracking)
        // {
        //     videoMonitor.StopTracking();
        //     orderingInLine.StartBlinkingTimer();
        //     stopedTracking = true;
        // }
    }
}
