using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManagerScript : MonoBehaviour
{
    public Toggle muteToggle;


    [SerializeField]
    AudioSource nonGameSound;

    // Start is called before the first frame update
    void Start()
    {
        int muter = PlayerPrefs.GetInt("Mute");
        muteToggle.isOn = muter == 1;

        Debug.Log(muter);

        nonGameSound.Play();
        if (muter == 1)
        {
            nonGameSound.mute = true;
        }
    }

    public void MuteToggle()
    {
        bool isMuted = muteToggle.isOn;
        PlayerPrefs.SetInt("Mute", isMuted ? 1 : 0);
        PlayerPrefs.Save();

        if (isMuted)
        {
            nonGameSound.mute = true;
        }
        else
        {
            nonGameSound.mute = false;
            if (!nonGameSound.isPlaying)
            {
                nonGameSound.Play();
            }
        }
    }
}
