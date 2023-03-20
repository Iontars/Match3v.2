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
/// <summary>
/// Отвечает за колличество токенов которые необходимо собрать для завершения карты, а тк же вывод этой информации на UI
/// </summary>
public class GoalManager : MonoBehaviour
{
    public BlankGoal[] levelGoals;
    public List<GoalPanel> currentGoals = new List<GoalPanel>(); // vid 40
    public GameObject goalPrefab;
    public GameObject goalIntroParent;
    public GameObject goalGameParent;
    EndGameManager EndGame;
    Board board;

    void SetupGoals()
    {
        for (int i = 0; i < levelGoals.Length; i++)
        {
            // создать панель целей в меню
            GameObject goal = Instantiate(goalPrefab, goalIntroParent.transform.position, Quaternion.identity, goalIntroParent.transform);
            goal.transform.SetParent(goalIntroParent.transform);
            // установить изображение и текст количество целей
            GoalPanel panel = goal.GetComponent<GoalPanel>();
            panel.thisSprite = levelGoals[i].goalSprite;
            panel.thisString = "0/" + levelGoals[i].numberNeeded;

            // создать панель целей на доске
            GameObject gameGoal = Instantiate(goalPrefab, goalGameParent.transform.position, Quaternion.identity, goalIntroParent.transform);
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
        // цикл проходится по массиву levelGoals и проверяет достигло ли текущее количество собранных токенос с целевым
        // если в одном из эллементов массива levelGoals это условие было соблюдено то счётчик goalsCompleted прибавляет единицу
        // эт означает что в одном из эллементов массива условие было солюдено и больше это эллемент проверяться не будет
        // как только все условия во всех эллементах будут соблюдены счётчик goalsCompleted станет равен levelGoals.Length
        // это создаст ивент успешного выполения задания
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
        if (goalsCompleted >= levelGoals.Length) // Победа  // создать ивент
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

    // метод достиженяи целей
    public void GetGoal()
    {
        if (board != null)
        {
            if (board.world != null)
            {
                if (board.Level < board.world.levels.Length)
                {
                    if (board.world.levels[board.Level] != null)
                    {
                        levelGoals = board.world.levels[board.Level].levelGoals;
                        // очень важная часть// установка нулевых значений в поле SO numberCollected иначе SO (Level) не обновит свои значения
                        for (int i = 0; i < levelGoals.Length; i++)
                        {
                            levelGoals[i].numberCollected = 0;
                        }
                    } 
                }
            }
        }
    }
    private void Awake()
    {
        EndGame = FindObjectOfType<EndGameManager>();
        board = FindObjectOfType<Board>();
    }

    void Start()
    {
        GetGoal();
        SetupGoals();
    }
    
}
