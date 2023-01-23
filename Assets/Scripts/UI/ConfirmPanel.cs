using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ConfirmPanel : MonoBehaviour
{
    public string levelToLoad;
    public Image[] stars;
    public int level;
    
    public void Cancel()
    {
        gameObject.SetActive(false);
    }

    public void Play()
    {
        //PlayerPrefs.SetInt("Current Level", level - 1); vid 45 (29min)
        PlayerPrefs.SetInt(PlayerPrefsStorage.keyCurrentLevel, level - 1); // улучшено
        SceneManager.LoadScene(levelToLoad);
    }

    void ActivateStars() // метод повторяется в скрипте LevelButton !!! рефактор
    {
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].enabled = false;
        }
    }

    void Start()
    {
        ActivateStars();
        
    }

    void Update()
    {
        
    }
}
