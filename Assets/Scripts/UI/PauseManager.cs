#region Using
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Linq;
using TMPro;
#endregion


/// <summary> Descriptions </summary>
public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel;
    private Board _board;
    public bool paused = false;
    public Image soundButton;
    public Sprite musicOnSprite;
    public Sprite musicOffSprite;
    private void Awake()
    {
        // в PlayerPrefs ключ "Sound" для звука, 0 - mute, 1 - unmute
        if (PlayerPrefs.HasKey("Sound"))
        {
            soundButton.sprite = PlayerPrefs.GetInt("Sound") == 0 ? musicOffSprite : musicOnSprite;
            
            // if (PlayerPrefs.GetInt("Sound") == 0)
            // {
            //     soundButton.sprite = musicOffSprite;
            // }
            // if (PlayerPrefs.GetInt("Sound") == 1)
            // {
            //     soundButton.sprite = musicOnSprite;
            // }
        }
        else
        {
            soundButton.sprite = musicOnSprite;
        }
        pausePanel.SetActive(false);
        _board = GameObject.FindWithTag("Board").GetComponent<Board>();
    }

    public void PauseGame()
    {
        paused = !paused;
    }

    public void ExitGame()
    {
        SceneManager.LoadScene("Splash");
    }

    public void SoundButton()
    {
        print(PlayerPrefs.GetInt("Sound"));
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 0)
            {
                soundButton.sprite = musicOnSprite;
                PlayerPrefs.SetInt("Sound", 1);
            }
            else
            {
                soundButton.sprite = musicOffSprite;
                PlayerPrefs.SetInt("Sound", 0);
            }
        }
        else
        {
            soundButton.sprite = musicOffSprite;
            PlayerPrefs.SetInt("Sound", 1);
        }
    }
    private void Start()
    {
        
    }


    private void Update()
    {
        if (paused && !pausePanel.activeInHierarchy)
        {
            pausePanel.SetActive(true);
            _board.currentState = GameState.pause;
        }
        if (!paused && pausePanel.activeInHierarchy)
        {
            pausePanel.SetActive(false);
            _board.currentState = GameState.move;
        }   
    }
}
