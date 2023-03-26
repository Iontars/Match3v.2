using Game_Data_Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace Base_Game_Scripts
{
    public class ScoreManager : MonoBehaviour
    {
        private Board _board;
        private GameData _gameData;
        private int _numberStars;
        public Image scoreBar;
        public Text scoreText;
        public int score;

        public void IncreaseScore(int amountToIncrease)
        {
            score += amountToIncrease;
            for (int i = 0; i < _board.scoreGoals.Length; i++)
            {
                if (score > _board.scoreGoals[i] && _numberStars < i + 1)
                {
                    _numberStars++;
                }
            }
            if (_gameData != null)
            {
                int highScore = _gameData.saveData.highScores[_board.Level];
                if (highScore < score)
                {
                    _gameData.saveData.highScores[_board.Level] = score;
                    //gameData.saveData.stars[board.level] = numberStars;
                }

                var currentStars = _gameData.saveData.stars[_board.Level];
                if (_numberStars > currentStars)
                {
                    _gameData.saveData.stars[_board.Level] = _numberStars;
                }
                _gameData.Save();
            }
            UpdateBar();
        }
        
        private void UpdateBar()
        {
            scoreText.text = score.ToString();
            if (_board != null && scoreBar != null)
            {
                int lenght = _board.scoreGoals.Length;
                scoreBar.fillAmount = (float)score / (float)_board?.scoreGoals[_board.scoreGoals.Length - 1];
            }
        }
        
        void Start()
        {
            _board = FindObjectOfType<Board>();
            _gameData = FindObjectOfType<GameData>();
        }
    }
}
