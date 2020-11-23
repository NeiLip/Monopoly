using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundHandler : MonoBehaviour
{
    public AudioSource TickAudio;
    public AudioSource SnatchSound;
    public AudioSource ButtonClickSound;


    public void PlayTick() {
        TickAudio.Play();
    }

    public void PlaySnatch() {
        SnatchSound.Play();
    }

    public void PlayButtonClick() {
        ButtonClickSound.Play();
    }
}
