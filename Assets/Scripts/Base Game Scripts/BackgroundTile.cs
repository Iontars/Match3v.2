using UnityEngine;

namespace Base_Game_Scripts
{
    public class BackgroundTile : MonoBehaviour
    {
        private SpriteRenderer _sprite;
        private GoalManager _goalManager;
        private bool _isGoalManagerNotNull;
        public int hitPoints;

        private void Awake()
        {
            _sprite = GetComponent<SpriteRenderer>();
            _goalManager = FindObjectOfType<GoalManager>();
            _isGoalManagerNotNull = _goalManager != null;
        }
        
        public void TakeDamage(int damage)
        {
            hitPoints -= damage;
            MakeLighter();
        }

        private void MakeLighter() // изменение цвета фоновой плитки п мере получения урона
        {
            Color color = _sprite.color;
            float newAlfa = color.a * .5f;
            _sprite.color = new Color(color.r, color.g, color.b, newAlfa);
        }


        private void CheckForBreakableTokens()
        {
            if (hitPoints <= 0)
            {
                // проверка на совпадение целей, если целью уровня стоят Breakable плитки
                if (_isGoalManagerNotNull)
                {
                    _goalManager.CompareGoal(gameObject.tag);
                    _goalManager.UpdateGoals();
                    print("Breakable tile was destroyed");
                }
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            CheckForBreakableTokens();
        }
    }
}
