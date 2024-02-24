using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfficerLegsAudioManager : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip[] clips = new AudioClip[4];
    /*0|Step1, 1|Step2, 2|Step3, 3|Step4
     */

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

    void PlayStep3()
    {
        audioSource.PlayOneShot(clips[2], footsteps);
    }

    void PlayStep4()
    {
        audioSource.PlayOneShot(clips[3], footsteps);
    }
}
