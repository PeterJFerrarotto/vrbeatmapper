using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongManager : MonoBehaviour
{

    private static SongManager _songManager;

    public static SongManager SongManagerObj
    {
        get
        {
            if (_songManager == null)
            {
                _songManager = new SongManager();
            }
            return _songManager;
        }
    }

    public GameObject AudioObject;

    public UnityEngine.UI.Slider TimeSlider;

    private float songDuration;
    private float songCurrentTime;
    // Start is called before the first frame update
    void Start()
    {
        songDuration = AudioObject.GetComponent<AudioSource>().clip.length;
        songCurrentTime = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayPauseSong()
    {
        if (AudioObject.GetComponent<AudioSource>().isPlaying)
        {

        } else
        {
            AudioObject.GetComponent<AudioSource>().Play();
        }
    }
}
