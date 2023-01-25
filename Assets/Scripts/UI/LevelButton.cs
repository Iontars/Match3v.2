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
    Image buttonImage;
    Button myButton;
    private int starsActive;

    [Header("Level UI")]
    public Image[] stars;
    public Text levelText;
    public int level;
    public GameObject confirmPanel;

    GameData gameData;


    // ������
    public void ConfirmPanel() // public void ConfirmPanel(int level) �������� ���������� �� ������
    {
        // vid 45 (27 ���)  ��������� ����� ����������� ��������//confirmPanel.GetComponent<ConfirmPanel>().level = level; // �������� ������ ���������� ������ ������ �� ������ ������ ������������� ������
        confirmPanel.GetComponent<ConfirmPanel>().level = this.level;
        confirmPanel.SetActive(true);
    }

    void LoadData()
    {
        // проверить есть ли файл данных
        if (gameData != null)
        {
            // решить активен ли уровень
            if (gameData.saveData.isActive[level - 1])
            {
                isActive = true;
            }
            else
            {
                isActive = false;
            }

            // решить сколько звезд активировать// загрузка из базы данных 
            starsActive = gameData.saveData.stars[level - 1];
        }
    }


    // ��������� ����������� ������ ������/. ��������� ������������ ������ ������ �������
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

    // �������� ����� ������ �� ������
    void ShowLevel()
    {
        levelText.text = "" + level;
    }

    // сколько звёзд активировать на данном объекте
    void ActivateStars()
    {
        for (int i = 0; i < starsActive; i++) 
        {
            stars[i].enabled = true;
        }
    }

    private void Awake()
    {
        gameData = FindObjectOfType<GameData>();
        buttonImage = GetComponent<Image>();
        myButton = GetComponent<Button>();
    }


    void Start()
    {
        LoadData();
        ActivateStars();
        DecideSprite();
        ShowLevel();
    }

    void Update()
    {

    }
}
