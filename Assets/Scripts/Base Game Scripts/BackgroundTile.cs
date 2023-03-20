using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTile : MonoBehaviour
{
    private SpriteRenderer sprite;
    public int hitPoints;
    GoalManager goalManager;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        goalManager = FindObjectOfType<GoalManager>();
    }
    public void TakeDamage(int damage)
    {
        hitPoints -= damage;
        MakeLighter();
    }

    void MakeLighter() // изменение цвета фоновой плитки п мере получения урона
    {
        Color color = sprite.color;
        float newAlfa = color.a * .5f;
        sprite.color = new Color(color.r, color.g, color.b, newAlfa);
    }


    private void CheckForBreakableTokens()
    {
        if (hitPoints <= 0)
        {
            // проверка на совпадение целей, если целью уровня стоят Breakable плитки
            if (goalManager != null)
            {
                goalManager.CompareGoal(gameObject.tag);
                goalManager.UpdateGoals();
                print("Breakable tile was destroyed");
            }
            Destroy(gameObject);
        }
    }
    void Update()
    {
        CheckForBreakableTokens();
    }
}
