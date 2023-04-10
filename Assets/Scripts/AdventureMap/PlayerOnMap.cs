#region Using

using System;
using System.Collections;
using Unity.VisualScripting;
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
        private int _number = 0;
        private int _numbersOfSteps = 0;
        private int _numberOfCurrentLevel;
        private readonly int _adjustmentForEventField = 1;
        private bool _isCanMove, _isStartMove;

        private Action _finishGame;

        
        // собака
        private void Awake()
        {
            _currentPlayerMapPosition = gameObject.transform;
        }

        private void Start()
        {
            _isCanMove = true;
        }

        private void OnEnable()
        {
            gameObject.transform.position = mapCheckPointsArray[_number].position;
            _finishGame += FinishMassage;
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

        public void GetMove()
        {
            if (_number <= mapCheckPointsArray.Length)
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
                //_numbersOfSteps = 6;
                print("Выброшено число " + _numbersOfSteps);
                StartCoroutine(MoveSteps(_numbersOfSteps));
            }
        }
        
        private void Update()
        {

            if (_isCanMove && _number < mapCheckPointsArray.Length)
            {
                MoveToPosition();
            }

            if (!_isStartMove)
            {
                _numberOfCurrentLevel = _number;
                //print("Текущая клетка " + _numberOfCurrentLevel);
                _isStartMove = true;
            }
            
            if (Input.GetKeyDown(KeyCode.Space) && _number <= mapCheckPointsArray.Length)
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
                //_numbersOfSteps = Random.Range(1, 4);
                _numbersOfSteps = 6;
                print("Выброшено число " + _numbersOfSteps);
                StartCoroutine(MoveSteps(_numbersOfSteps));
            }
        }

        private IEnumerator MoveSteps(int steps)
        {
            for (int i = 0; i < steps; i++)
            {
                _number++;
                if (_number >= mapCheckPointsArray.Length)
                {
                    _finishGame?.Invoke();
                    break;
                }
                print(_number);
                //print("Move To position: " + _number );
                _isCanMove = true;
                yield return new WaitForSecondsRealtime(0.4f);
                _isCanMove = false;
                
            }
            print(_number +"______" + mapCheckPointsArray.Length);
            _isStartMove = false;
        }

        private void FinishMassage()
        {
            Debug.LogWarning("FinishGame");
        }
    }
}
