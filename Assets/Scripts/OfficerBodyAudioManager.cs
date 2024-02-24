using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfficerBodyAudioManager : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip[] clips = new AudioClip[12];
    /*  0|Swing1, 1|Swing2, 2|Swing3
     *  3|Blood1, 4|Blood2, 5|Blood3
     *  6|Counter1
     *  7|Death
     *  8|Thump1, 9|Thump2, 10|Thump3, 
     *  11|SwitchToMelee
     */

    void Start()
    {
        audioSource = transform.GetComponent<AudioSource>();
    }

    void Swing()
    {
        audioSource.PlayOneShot(clips[Random.Range(0, 3)], 1.5f);
    }

    void Blood()
    {
        audioSource.PlayOneShot(clips[Random.Range(3, 6)], 0.5f);
    }

    void Counter()
    {
        audioSource.PlayOneShot(clips[6], 1);
    }

    void Death()
    {
        audioSource.PlayOneShot(clips[7], 1);
    }

    void Thump()
    {
        audioSource.PlayOneShot(clips[Random.Range(8, 11)], 1.5f);
    }

    void Switch()
    {
        audioSource.PlayOneShot(clips[11], 0.4f);
    }
}
