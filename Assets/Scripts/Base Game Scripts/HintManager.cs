using System.Collections.Generic;
using UnityEngine;

namespace Base_Game_Scripts
{
    public class HintManager : MonoBehaviour
    {
        private Board _board;
        private float _hintDelaySeconds;
        public float hintDelay;
        public GameObject hintParticle;
        public GameObject currentHint;

        private void Start()
        {
            _board = FindObjectOfType<Board>();
            _hintDelaySeconds = hintDelay;
        }

        // найти все возможные совпадения на доске
        private List<GameObject> FindAllMatches()
        {
            List<GameObject> possibleMoves = new List<GameObject>();
            for (int i = 0; i < _board.Width; i++)
            {
                for (int j = 0; j < _board.Height; j++)
                {
                    if (_board.currentLevelAllTokensArray[i, j] != null)
                    {
                        if (i < _board.Width - 1)
                        {
                            if (_board.SwitchAndCheck(i, j, Vector2.right))
                            {
                                possibleMoves.Add(_board.currentLevelAllTokensArray[i,j]) ;
                            }
                        }
                        if (j < _board.Height - 1)
                        {
                            if (_board.SwitchAndCheck(i, j, Vector2.up))
                            {
                                possibleMoves.Add(_board.currentLevelAllTokensArray[i, j]);
                            }
                        }
                    }
                }
            }
            return possibleMoves;
        }
        // выбрать одно из этих совпадений случайным образом
        private GameObject PickOneRandomly()
        {
            List<GameObject> possibleMoves = FindAllMatches();
            if (possibleMoves.Count > 0)
            {
                int pieceToUse = Random.Range(0, possibleMoves.Count);
                return possibleMoves[pieceToUse];
            }
            else return null;
        }
        // создать подсказку на выбранном совпадении
        void MarkHint()
        {
            GameObject move = PickOneRandomly();
            if (move != null)
            {
                currentHint = Instantiate(hintParticle, move.transform.position, Quaternion.identity);
            }
        }
        // уничтожить подсказку
        public void DestroyHint()
        {
            if (currentHint != null)
            {
                Destroy(currentHint);
                currentHint = null;
                _hintDelaySeconds = hintDelay;
            }
        }

        void Update()
        {
            if (_board.currentState == GameState.Move)
            {

                _hintDelaySeconds -= Time.deltaTime;
                if (_hintDelaySeconds <= 0 && currentHint == null)
                {
                    MarkHint();
                    _hintDelaySeconds = hintDelay;

                }
            }
        }
    }
}
