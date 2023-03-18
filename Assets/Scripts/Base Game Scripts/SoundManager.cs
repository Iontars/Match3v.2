using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource[] destroyNoise;
    
    public void PlayRandomDestroyNoise()
    {
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 1)
            {
                int clipToplay = Random.Range(0, destroyNoise.Length);
                destroyNoise[clipToplay].Play();
            }
        }
    }
}
