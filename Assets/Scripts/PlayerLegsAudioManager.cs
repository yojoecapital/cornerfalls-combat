using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLegsAudioManager : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip[] clips = new AudioClip[2]; // 0|Step1, 1|Step2, 

    public float footsteps = 0.3f;

    void Start()
    {
        audioSource = transform.GetComponent<AudioSource>();
    }

    void PlayStep1()
    {
        audioSource.PlayOneShot(clips[0], footsteps);
    }

    void PlayStep2()
    {
        audioSource.PlayOneShot(clips[1], footsteps);
    }
}
