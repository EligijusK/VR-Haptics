using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Video;

namespace DefaultNamespace
{
    public class VideoClipController : MonoBehaviour
    {
        [SerializeField] VideoClip _defaultVideoClip;
        [SerializeField] VideoClipData[] _videoClips;
        
        public VideoClip GetVideoClip(int index)
        {
            if (_defaultVideoClip != null && index == -1)
            {
                return _defaultVideoClip;
            }
            foreach (var videoClipData in _videoClips)
            {
                if (videoClipData.index == index)
                {
                    return videoClipData._videoClip;
                }
            }

            return null;
        }
    }

    [System.Serializable]
    public class VideoClipData
    {
        public int index;
        public VideoClip _videoClip;
    }
    
}