using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSW_SoundManager : MonoBehaviour
{
    public enum ESoundType
    {
        EFT_To3D,
        EFT_DoorSound,
        EFT_FuniSound,
        EFT_ButtonSound1,
        EFT_ButtonSound2,
        EFT_ImageSound,
        EFT_PetSound,
        EFT_FuniMoveSound
    }

    static JSW_SoundManager instance;
    public static JSW_SoundManager Get()
    {

        if (instance == null)
        {
            GameObject soundManager = GameObject.Find("SoundManager");
            instance = soundManager.GetComponent<JSW_SoundManager>();
        }
        return instance;
    }


    // audiosource
    public AudioSource eftAudio;

    public AudioClip[] eftAudios;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;


            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayEftSound(ESoundType idx)
    {
        int audioIdx = (int)idx;
        eftAudio.PlayOneShot(eftAudios[audioIdx]);
        //eftAudio.clip = eftAudios[audioIdx];
        //eftAudio.Play();
    }
}
