using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoonSoundManagerLogin : MonoBehaviour
{
    public AudioSource audioSource;  // AudioSource 컴포넌트
    public AudioClip[] hoonAudioClipArray;
    public AudioClip[] jswAudioClopAttay;
    public AudioClip[] smjAudioClopAttay;
    // Start is called before the first frame update
    void Start()
    {

        audioSource = transform.GetComponent<AudioSource>();
        if (audioSource != null)
        {
            print("findAudioSource");
        }

    }

    // Update is called once per frame
    /*void Update()
    {
        
    }*/

    public void PlaySound(int idx)
    {
        audioSource.clip = hoonAudioClipArray[idx];
        audioSource.Play();

    }
    public void PlaySound(string name, int idx)
    {
        if (name == "hoonAudioClipArray")
        {
            audioSource.clip = hoonAudioClipArray[idx];
            audioSource.Play();

        }

        if (name == "jswAudioClopAttay")
        {
            audioSource.clip = jswAudioClopAttay[idx];
            audioSource.Play();

        }

        if (name == "smjAudioClopAttay")
        {
            //audioSource.clip = smjAudioClopAttay[idx];
            //audioSource.Play();

        }


    }
} //클래스끝
