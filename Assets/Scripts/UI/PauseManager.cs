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
    private SoundManager sound; // убрать здесь ему не место
    private void Awake()
    {
        sound = FindObjectOfType<SoundManager>();
        _board = GameObject.FindWithTag("Board").GetComponent<Board>();
    }

    private void Start()
    {
        // в PlayerPrefs ключ "Sound" для звука, 0 - mute, 1 - unmute
        pausePanel.SetActive(false);
        if (PlayerPrefs.HasKey("Sound"))
        {
            soundButton.sprite = PlayerPrefs.GetInt("Sound") == 0 ? musicOffSprite : musicOnSprite;
        }
        else
        {
            soundButton.sprite = musicOnSprite;
        }
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
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 0)
            {
                soundButton.sprite = musicOnSprite;
                PlayerPrefs.SetInt("Sound", 1);
                sound.AdjustValue();
            }
            else
            {
                soundButton.sprite = musicOffSprite;
                PlayerPrefs.SetInt("Sound", 0);
                sound.AdjustValue();
            }
        }
        else
        {
            soundButton.sprite = musicOffSprite;
            PlayerPrefs.SetInt("Sound", 1);
            sound.AdjustValue();
        }
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
