using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoonSoundManagerLogin : MonoBehaviour
{
    public AudioSource audioSource;  // AudioSource ÄÄÆ÷³ÍÆ®
    public AudioClip[] audioClipArray;
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
        audioSource.clip = audioClipArray[idx];
        audioSource.Play();
       
    }

}
