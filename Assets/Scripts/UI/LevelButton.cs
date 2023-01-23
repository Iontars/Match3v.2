using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [Header("Active Stuff")]
    public bool isActive;
    public Sprite activeSprite;
    public Sprite lockedSprite; 
    public Image[] stars;
    public Text levelText;
    public int level;
    public GameObject confirmPanel;

    Image buttonImage;
    Button myButton;

    // кнопка
    public void ConfirmPanel() // public void ConfirmPanel(int level) вводится пераметром на кнопке
    {
        // vid 45 (27 мин)  переделал более оптимальным способом//confirmPanel.GetComponent<ConfirmPanel>().level = level; // передача номера запущеного уровня дальше на панель Панель подтверждения уровня
        confirmPanel.GetComponent<ConfirmPanel>().level = this.level;
        confirmPanel.SetActive(true);
    }

    // проверяет доступность выбора уровня/. блокирует разблокирует кнопки выбора уровней
    void DecideSprite()
    {
        if (isActive)
        {
            buttonImage.sprite = activeSprite;
            myButton.enabled = true;
            levelText.enabled= true;
         }
        else
        {
            buttonImage.sprite = lockedSprite;
            myButton.enabled = false;
            levelText.enabled= false;
        }
    }

    // показать номер уровня на кнопке
    void ShowLevel()
    {
        levelText.text = "" + level;
    }

    // показать звёзды под кнопкой выбора уровня 
    void ActivateStars()
    {
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].enabled = false;
        }
    }

    private void Awake()
    {
        buttonImage = GetComponent<Image>();
        myButton= GetComponent<Button>();
    }


    void Start()
    {
        ActivateStars();
        DecideSprite();
        ShowLevel();
    }

    void Update()
    {

    }
}
