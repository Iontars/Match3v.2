using System;
using System.Collections;
using UnityEngine;
using Static_Prefs;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using System.Linq;

namespace Adventure.AdventureMap
{
    public class PlayerOnMapPro : MonoBehaviour
    {
        private GlobalMapController _globalMapController;
        private float _playerMapSpeed;
        private int _numberOfCurrentLevel;
        [HideInInspector] public Vector2 currentPlayerMapPosition;
        [HideInInspector] public int maxPointsCount;
        [HideInInspector] public bool isLerpMoving;
        
        private void Awake()
        {
            _globalMapController = FindObjectOfType<GlobalMapController>();
            gameObject.transform.position = currentPlayerMapPosition;
        }

        private void Start()
        {
            isLerpMoving = true;
        }

        private void OnEnable()
        {
            _globalMapController.rollCube.NumberReceived += MoveStepByStepsHandler;
            foreach (var item in _globalMapController.mapCheckPointsArray)
            {
                if (item != null)
                {
                    item.GetComponent<CheckPoint_S>().Action += MoveTeleportToPointHandler;
                }
            }
        }
        
        private void OnDisable()
        {
            _globalMapController.rollCube.NumberReceived -= MoveStepByStepsHandler;
            foreach (var item in _globalMapController.mapCheckPointsArray)
            {
                if (item != null)
                {
                    item.GetComponent<CheckPoint_S>().Action -= MoveTeleportToPointHandler;
                }
            }
        }

        private void MoveToPosition()
        {
            transform.position = Vector2.Lerp(transform.position,
                _globalMapController.mapCheckPointsArray[PlayerPrefs.GetInt(PlayerPrefsStorage.PlayerCurrentPositionOnMap)].transform.position,
                0.05f);
        }

        private void Update()
        {
            if (isLerpMoving && PlayerPrefs.GetInt(PlayerPrefsStorage.PlayerCurrentPositionOnMap) < maxPointsCount)
            {
                MoveToPosition();
            }
        }

        public void MoveStepByStepsHandler(int x)
        {
            CheckSpecialMapPosition();
            StartCoroutine(MoveStepBySteps(x));
        }

        public void MoveTeleportToPointHandler(int x)
        {
            CheckSpecialMapPosition();
            StartCoroutine(MoveTeleportToPoint(x));
        }
        
        private IEnumerator MoveStepBySteps(int steps)
        {
            for (int i = 0; i < steps; i++)
            {
                PlayerPrefs.SetInt(PlayerPrefsStorage.PlayerCurrentPositionOnMap,
                    PlayerPrefs.GetInt(PlayerPrefsStorage.PlayerCurrentPositionOnMap) + 1);
                isLerpMoving = true;
                yield return new WaitForSeconds(PlayerPrefsStorage.CoroutineForLerpDelay);
                isLerpMoving = false;
            }

            print(PlayerPrefs.GetInt(PlayerPrefsStorage.PlayerCurrentPositionOnMap) + " ИЗ " + maxPointsCount);
            PlayerPrefs.SetInt(PlayerPrefsStorage.KeyCurrentLevel,
                PlayerPrefs.GetInt(PlayerPrefsStorage.PlayerCurrentPositionOnMap));
            StartCoroutine(MoveIsCompleted());
        }
        
        private IEnumerator MoveTeleportToPoint(int targetPoint) 
        {
            PlayerPrefs.SetInt(PlayerPrefsStorage.PlayerCurrentPositionOnMap, targetPoint);
            isLerpMoving = true;
            yield return new WaitForSeconds(PlayerPrefsStorage.CoroutineForLerpDelay);
            isLerpMoving = false;
            print(PlayerPrefs.GetInt(PlayerPrefsStorage.PlayerCurrentPositionOnMap) + " ИЗ " + maxPointsCount);
            StartCoroutine(MoveIsCompleted()); 
        }
        
        private static IEnumerator MoveIsCompleted() 
        {
            yield return new WaitForSeconds(0.3f);
            print("Движение завершилось");
            //SceneManager.LoadScene("Main");
        }

        private void CheckSpecialMapPosition()
        {
            if (PlayerPrefs.GetInt(PlayerPrefsStorage.PlayerCurrentPositionOnMap) == 4)
            {
                PlayerPrefs.SetInt(PlayerPrefsStorage.PlayerCurrentPositionOnMap, 1 - GlobalMapController.AdjustmentForEventField);
            }
            if (PlayerPrefs.GetInt(PlayerPrefsStorage.PlayerCurrentPositionOnMap) == 7)
            {
                PlayerPrefs.SetInt(PlayerPrefsStorage.PlayerCurrentPositionOnMap, 4 - GlobalMapController.AdjustmentForEventField);
            }
        }
        
        private void FinishMassage()
        {
            Debug.LogWarning("FinishGame");
            currentPlayerMapPosition = gameObject.transform.position;
            PlayerPrefs.GetInt(PlayerPrefsStorage.PlayerCurrentPositionOnMap, 0);
            isLerpMoving = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            print("Вошёл в точку" + other.gameObject.name);
        }
    }
}