using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectManager : MonoBehaviour
{
    public GameObject[] panels;
    public GameObject currentPanel;
    public int page;
    private GameData _gameData;
    public int currentLevel = default;

    private void Awake()
    {
        _gameData = FindObjectOfType<GameData>();
        
    }

    private void Start()
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(false);
        }

        if (_gameData != null)
        {
            for (int i = 0; i < _gameData.saveData.isActive.Length; i++)
            {
                if (_gameData.saveData.isActive[i])
                {
                    currentLevel = i;
                }
            }
        }

        page = (int)Mathf.Floor(currentLevel / 9f);
        currentPanel = panels[page];
        panels[page].SetActive(true);
    }

    public void PageRight()
    {
        if (page < panels.Length - 1)
        {
            currentPanel.SetActive(false);
            page++;
            currentPanel = panels[page];
            currentPanel.SetActive(true);

        }
    }
    public void PageLeft()
    {
        if (page > 0)
        {
            currentPanel.SetActive(false);
            page--;
            currentPanel = panels[page];
            currentPanel.SetActive(true);

        }
    }
    private void Update()
    {
        
    }
}
