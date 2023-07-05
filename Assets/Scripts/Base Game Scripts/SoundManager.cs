using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Base_Game_Scripts
{
    public class SoundManager : MonoBehaviour
    {
        public AudioSource[] destroyNoise;
        public AudioSource backgroundMusic;

        private void Start()
        {
            if (PlayerPrefs.HasKey("Sound"))
            {
                if (PlayerPrefs.GetInt("Sound") == 0)
                {
                    backgroundMusic.volume = 0;
                }
                else
                {
                    backgroundMusic.Play();
                    backgroundMusic.volume = 1;
                }
            }
            else
            {
                PlayerPrefs.SetInt("Sound", 1);
                backgroundMusic.Play();
                backgroundMusic.volume = 1;
            }
        }

        public void AdjustValue()
        {
            if (PlayerPrefs.HasKey("Sound"))
            {
                if (PlayerPrefs.GetInt("Sound") == 0)
                {
                    backgroundMusic.volume = 0;
                }
                else
                {
                    backgroundMusic.Play();
                    backgroundMusic.volume = 1;
                }
            }
        }

        public void PlayRandomDestroyNoise()
        {
            if (PlayerPrefs.HasKey("Sound"))
            {
                if (PlayerPrefs.GetInt("Sound") == 1)
                {
                    int clipTopPlay = Random.Range(0, destroyNoise.Length);
                    destroyNoise[clipTopPlay].Play();
                }
            }
        }

        private void Update()
        {
            print(PlayerPrefs.GetInt("Sound") + " звук");
        }
    }
}
