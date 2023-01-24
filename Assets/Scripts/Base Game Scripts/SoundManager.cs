using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource[] destroyNoise;


    public void PlayRandomDestroyNoise()
    {
        int clipToplay = Random.Range(0, destroyNoise.Length);
        destroyNoise[clipToplay].Play();
    }
}
