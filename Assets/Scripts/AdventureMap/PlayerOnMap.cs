#region Using

using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

#endregion

namespace AdventureMap
{
    /// <summary> Descriptions </summary>
    public class PlayerOnMap : MonoBehaviour
    {
        private Transform _currentPlayerMapPosition;
        public Transform[] mapCheckPointsArray;
        private float _playerMapSpeed;
        public int _number = 0;
        private int _numbersOfSteps = 0;
        private int _numberOfCurrentLevel;
        private int _adjustmentForEventField = 1;
        private bool _isCanMove, _isStartMove;
        void Awake()
        {
            _currentPlayerMapPosition = gameObject.transform;
        }

        void Start()
        {
        
        }

        private void OnEnable()
        {
            gameObject.transform.position = mapCheckPointsArray[_number].position;
        }

        private void OnMouseDown()
        {
            _isCanMove = true;
            _number++;
            _isCanMove = false;
        }

        private void MoveToPosition()
        {
            transform.position = Vector2.Lerp(transform.position, mapCheckPointsArray[_number].position, 0.3f);
        }
        void Update()
        {

            if (_isCanMove)
            {
                MoveToPosition();
            }

            if (!_isStartMove)
            {
                _numberOfCurrentLevel = _number;
                print("Текущая клетка " + _numberOfCurrentLevel);
                _isStartMove = true;
            }
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (_numberOfCurrentLevel == 4)
                {
                    _number = 1 - _adjustmentForEventField;
                }
                if (_numberOfCurrentLevel == 7)
                {
                    _number = 4 - _adjustmentForEventField;
                }
                _isStartMove = true;
                _numbersOfSteps = Random.Range(1, 4);
                //_numbersOfSteps = 7;
                print("Выброшено число " + _numbersOfSteps);
                StartCoroutine(MoveSteps(_numbersOfSteps));
            }
        }

        private IEnumerator MoveSteps(int steps)
        {
            for (int i = 0; i < steps; i++)
            {
                _number++;
                print("Move To position: " + _number );
                _isCanMove = true;
                yield return new WaitForSecondsRealtime(0.4f);
                _isCanMove = false;
            }

            _isStartMove = false;
        }
        
    }
}
