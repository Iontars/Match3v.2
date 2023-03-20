using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    private Board board;
    public Text scoreText;
    public int score;
    public Image scoreBar;
    GameData gameData;
    private int numberStars;

    public void IncreaseScore(int AmountToIncrease)
    {
        score += AmountToIncrease;
        for (int i = 0; i < board.scoreGoals.Length; i++)
        {
            if (score > board.scoreGoals[i] && numberStars < i + 1)
            {
                numberStars++;
            }
        }
        if (gameData != null)
        {
            int highScore = gameData.saveData.highScores[board.Level];
            if (highScore < score)
            {
                gameData.saveData.highScores[board.Level] = score;
                //gameData.saveData.stars[board.level] = numberStars;
            }

            var currentStars = gameData.saveData.stars[board.Level];
            if (numberStars > currentStars)
            {
                gameData.saveData.stars[board.Level] = numberStars;
            }
            gameData.Save();
        }
        UpdateBar();
    }
    
    // private void OnApplicationPause()
    // {
    //     if (gameData != null)
    //     {
    //         gameData.saveData.stars[board.level] = numberStars;
    //     }
    //     gameData.Save();
    // }

    private void UpdateBar()
    {
        scoreText.text = score.ToString();
        if (board != null && scoreBar != null)
        {
            int lenght = board.scoreGoals.Length;
            scoreBar.fillAmount = (float)score / (float)board?.scoreGoals[board.scoreGoals.Length - 1];
        }
    }
    

    void Start()
    {
        board = FindObjectOfType<Board>();
        gameData = FindObjectOfType<GameData>();
    }

    void Update()
    {
        // scoreText.text = score.ToString();
        //
        // if (scoreBar != null)
        // {
        //     //int lenght = board.scoreGoals.Length;
        //     scoreBar.fillAmount = (float)score / (float)board?.scoreGoals[board.scoreGoals.Length - 1];
        // }
    }
}
