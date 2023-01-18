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

    public void IncreaseScore(int AmountToIncrease)
    {
        score += AmountToIncrease;
    }


    void Start()
    {
        board = FindObjectOfType<Board>();
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
