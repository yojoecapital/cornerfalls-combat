using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBodyAudioController : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip[] clips = new AudioClip[8];
    /*  0|Swing1, 1|Swing2, 2|Swing3
     *  3|Blood1, 4|Blood2, 5|Blood3
     *  6|Block1
     *  7|Death
     */

    void Start()
    {
        audioSource = transform.GetComponent<AudioSource>();
    }

    void Swing()
    {
        audioSource.PlayOneShot(clips[Random.Range(0,2)], 1);
    }

    void Blood()
    {
        audioSource.PlayOneShot(clips[Random.Range(3, 5)], 0.5f);
    }

    void Block()
    {
        audioSource.PlayOneShot(clips[6], 1);
    }

    void Death()
    {
        audioSource.PlayOneShot(clips[7], 1);
    }
}
