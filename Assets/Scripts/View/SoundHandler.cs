using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundHandler : MonoBehaviour
{
    public AudioSource TickAudio;//Called when animating a die roll
    public AudioSource SnatchSound;//Called when animating money changes

    public AudioSource ButtonClickSound;//Called when clicking buttons in menu


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
