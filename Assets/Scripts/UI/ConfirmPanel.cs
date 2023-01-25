using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ConfirmPanel : MonoBehaviour
{
    [Header("Level information")]
    public string levelToLoad;
    public int level;
    GameData gameData;
    int starsActive;
    int highScore;

    [Header("UI Stuff")]
    public Image[] stars;
    public Text highScoreText;
    public Text starText;


    private void Awake()
    {
        
    }

    public void Cancel()
    {
        gameObject.SetActive(false);
    }

    public void Play()
    {
        //PlayerPrefs.SetInt("Current Level", level - 1); vid 45 (29min)
        PlayerPrefs.SetInt(PlayerPrefsStorage.keyCurrentLevel, level - 1); // ��������
        SceneManager.LoadScene(levelToLoad);
    }

    void ActivateStars() // дублирующий метод из LevelButton !!! ИСПРАВИТЬ
    {
        for (int i = 0; i < starsActive; i++)
        {
            stars[i].enabled = true;
        }
    }

    void LoadData() // дублирующий метод из LevelButton !!! ИСПРАВИТЬ
    {
        if (gameData != null)
        {
            starsActive = gameData.saveData.stars[level - 1];
            highScore = gameData.saveData.highScores[level - 1];          
        }
    }

    void SetText()
    {
        highScoreText.text ="" + highScore;
        starText.text = "" + starsActive + "/3";
    }

    void Start()
    {
        

    }

    private void OnEnable()
    {
        gameData = FindObjectOfType<GameData>();
        LoadData();
        ActivateStars();
        SetText();
    }

    void Update()
    {
        
    }
}
