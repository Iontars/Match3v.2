#region Using

using System;
using System.Collections;
using Static_Prefs;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

#endregion

namespace Adventure.AdventureMap
{
    /// <summary> Descriptions </summary>
    public class PlayerOnMap : MonoBehaviour
    {
        public Transform[] mapCheckPointsArray;
        private Transform _currentPlayerMapPosition;
        private float _playerMapSpeed;
        private int _numbersOfSteps = 0;
        private int _numberOfCurrentLevel;
        private readonly int _adjustmentForEventField = 1;
        private bool _isLerpMoving, _isStartMove;

        private Action _finishGame;
        
        private void Awake()
        {
            print(PlayerPrefs.GetInt(PlayerPrefsStorage.PlayerCurrentPositionOnMap)+ "Landing on map");
            _currentPlayerMapPosition = gameObject.transform;
        }

        private void Start()
        {
            _isLerpMoving = true;
            print(PlayerPrefs.GetInt(PlayerPrefsStorage.PlayerCurrentPositionOnMap)+ "куда идём");
            
        }

        private void OnEnable()
        {
            gameObject.transform.position = mapCheckPointsArray[PlayerPrefs.GetInt(PlayerPrefsStorage.PlayerCurrentPositionOnMap)].position;
            _finishGame += FinishMassage;
        }
        
        private void MoveToPosition()
        {
            transform.position = Vector2.Lerp(transform.position, mapCheckPointsArray[PlayerPrefs.GetInt(PlayerPrefsStorage.PlayerCurrentPositionOnMap)].position, 0.03f);
        }
        
        private void Update()
        {
            //print(PlayerPrefs.GetInt(PlayerPrefsStorage.KeyCurrentLevel) + "Prefs");
        
            if (_isLerpMoving && PlayerPrefs.GetInt(PlayerPrefsStorage.PlayerCurrentPositionOnMap) < mapCheckPointsArray.Length)
            {
                MoveToPosition();
            }
            if (Input.GetKeyDown(KeyCode.Space) && PlayerPrefs.GetInt(PlayerPrefsStorage.PlayerCurrentPositionOnMap) <= mapCheckPointsArray.Length && !_isStartMove)
            {
                _isStartMove = true;
                if (PlayerPrefs.GetInt(PlayerPrefsStorage.PlayerCurrentPositionOnMap) == 4)
                {
                    PlayerPrefs.SetInt(PlayerPrefsStorage.PlayerCurrentPositionOnMap, 1 - _adjustmentForEventField);
                }
                if (PlayerPrefs.GetInt(PlayerPrefsStorage.PlayerCurrentPositionOnMap) == 7)
                {
                    PlayerPrefs.SetInt(PlayerPrefsStorage.PlayerCurrentPositionOnMap, 4 - _adjustmentForEventField);
                }
                _isStartMove = true;
                _numbersOfSteps = Random.Range(1, 4);
                //_numbersOfSteps = 1;
                print("Выброшено число " + _numbersOfSteps);
                StartCoroutine(MoveSteps(_numbersOfSteps));
            }
        }

        private IEnumerator MoveSteps(int steps)
        {
            for (int i = 0; i < steps; i++)
            {
                PlayerPrefs.SetInt(PlayerPrefsStorage.PlayerCurrentPositionOnMap, PlayerPrefs.GetInt(PlayerPrefsStorage.PlayerCurrentPositionOnMap) + 1 );
                if (PlayerPrefs.GetInt(PlayerPrefsStorage.PlayerCurrentPositionOnMap) >= mapCheckPointsArray.Length)
                {
                    _finishGame?.Invoke();
                    break;
                }
                //print(_number);
                //print("Move To position: " + _number );
                _isLerpMoving = true;
                yield return new WaitForSeconds(1f);
                _isLerpMoving = false;
                
            }
            print(PlayerPrefs.GetInt(PlayerPrefsStorage.PlayerCurrentPositionOnMap) +"______" + mapCheckPointsArray.Length);
            PlayerPrefs.SetInt(PlayerPrefsStorage.PlayerCurrentPositionOnMap, PlayerPrefs.GetInt(PlayerPrefsStorage.PlayerCurrentPositionOnMap));
            _isStartMove = false;
            PlayerPrefs.SetInt(PlayerPrefsStorage.KeyCurrentLevel, PlayerPrefs.GetInt(PlayerPrefsStorage.PlayerCurrentPositionOnMap) );
            //SceneManager.LoadScene("Main");
        }
        
        private void FinishMassage()
        {
            Debug.LogWarning("FinishGame");
            _currentPlayerMapPosition = gameObject.transform;
            PlayerPrefs.GetInt(PlayerPrefsStorage.PlayerCurrentPositionOnMap, 0);
            _isLerpMoving = true;
        }
    }
}
