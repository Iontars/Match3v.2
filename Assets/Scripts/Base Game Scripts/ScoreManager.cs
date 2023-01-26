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

    public void IncreaseScore(int AmountToIncrease)
    {
        score += AmountToIncrease;
        if (gameData != null)
        {
            int highScore = gameData.saveData.highScores[board.level];
            if (highScore < score)
            {
                gameData.saveData.highScores[board.level] = score;

            }
            gameData.Save();
        }
    }


    void Start()
    {
        board = FindObjectOfType<Board>();
        gameData = FindObjectOfType<GameData>();
    }

    void Update()
    {
        scoreText.text = score.ToString();

        if (scoreBar != null)
        {
            //int lenght = board.scoreGoals.Length;
            scoreBar.fillAmount = (float)score / (float)board?.scoreGoals[board.scoreGoals.Length - 1];
        }
    }
}
