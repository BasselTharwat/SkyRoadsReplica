using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartAudio : MonoBehaviour
{
    [SerializeField]
    AudioSource nonGameSound;

    private int muter;
    // Start is called before the first frame update
    void Start()
    {
        muter = PlayerPrefs.GetInt("Mute");
        if (!nonGameSound.isPlaying && muter != 1)
        {
            nonGameSound.Play();
        }

    }
}
