using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class BlankGoal
{
    public int numberNeeded;
    public int numberCollected;
    public Sprite goalSprite;
    public string MatchValue;
}

public class GoalManager : MonoBehaviour
{
    public BlankGoal[] levelGoals;
    public List<GoalPanel> currentGoals = new List<GoalPanel>(); // vid 40
    public GameObject goalPrefab;
    public GameObject goalIntoParent;
    public GameObject goalGameParent;
    EndGameManager EndGame;

    void SetupGoals()
    {
        for (int i = 0; i < levelGoals.Length; i++)
        {
            // создать панель целей в меню
            GameObject goal = Instantiate(goalPrefab, goalIntoParent.transform.position, Quaternion.identity);
            goal.transform.SetParent(goalIntoParent.transform);
            // установить изображение и текст количество целей
            GoalPanel panel = goal.GetComponent<GoalPanel>();
            panel.thisSprite = levelGoals[i].goalSprite;
            panel.thisString = "0/" + levelGoals[i].numberNeeded;

            // создать панель целей на доске
            GameObject gameGoal = Instantiate(goalPrefab, goalGameParent.transform.position, Quaternion.identity);
            gameGoal.transform.SetParent(goalGameParent.transform);
            panel = gameGoal.GetComponent<GoalPanel>();
            currentGoals.Add(panel); // vid 40
            panel.thisSprite = levelGoals[i].goalSprite;
            panel.thisString = "0/" + levelGoals[i].numberNeeded;
        }
    }

    // цели карты
    public void UpdateGoals() // vid 40
    {
        int goalsCompleted = default;
        for (int i = 0; i < levelGoals.Length; i++)
        {
            currentGoals[i].thisText.text = "" + levelGoals[i].numberCollected + "/"
                                               + levelGoals[i].numberNeeded;
            // ограничение набранных совпадений для целей уровня
            if (levelGoals[i].numberCollected >= levelGoals[i].numberNeeded)
            {
                goalsCompleted++;
                currentGoals[i].thisText.text = "" + levelGoals[i].numberNeeded + "/"
                                                   + levelGoals[i].numberNeeded;
            }
        }
        if (goalsCompleted >= levelGoals.Length) // Победа
        {
            if (EndGame != null)
            {
                EndGame.WinGame();
            }
            print("Win");
        }
    }

    public void CompareGoal(string goalToCompare) // ?убрать теговую систему
    {
        print(goalToCompare);
        // проверить совпадение плиток по тегам
        for (int i = 0; i < levelGoals.Length; i++)
        {
            if (goalToCompare == levelGoals[i].MatchValue)
            {
                levelGoals[i].numberCollected++;
            }
        }
    }

    private void Awake()
    {
        EndGame = FindObjectOfType<EndGameManager>();
    }

    void Start()
    {
        SetupGoals();
    }
    
}
