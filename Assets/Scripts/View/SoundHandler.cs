using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundHandler : MonoBehaviour
{

    public AudioSource TickAudio;


    public void PlayTick() {
        TickAudio.Play();
    }
}
