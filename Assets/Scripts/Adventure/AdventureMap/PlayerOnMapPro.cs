using System.Collections;
using UnityEngine;
using Static_Prefs;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Adventure.AdventureMap
{
    public class PlayerOnMapPro : MonoBehaviour
    {
        private GlobalMapController globalMapController;
        private float _playerMapSpeed;
        private int _numberOfCurrentLevel;
        [HideInInspector] public Vector2 currentPlayerMapPosition;
        [HideInInspector] public int maxPointsCount;
        [HideInInspector] public bool isLerpMoving;
        
        private void Awake()
        {
            globalMapController = FindObjectOfType<GlobalMapController>();
            gameObject.transform.position = currentPlayerMapPosition;
        }

        private void Start()
        {
            isLerpMoving = true;
        }
        
        private void MoveToPosition()
        {
            transform.position = Vector2.Lerp(transform.position,
                globalMapController.mapCheckPointsArray[PlayerPrefs.GetInt(PlayerPrefsStorage.PlayerCurrentPositionOnMap)].transform.position,
                0.03f);
        }

        private void Update()
        {
            if (isLerpMoving && PlayerPrefs.GetInt(PlayerPrefsStorage.PlayerCurrentPositionOnMap) < maxPointsCount)
            {
                MoveToPosition();
            }
        }

        public IEnumerator MoveStepBySteps(int steps)
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

        public IEnumerator MoveTeleportToPoint(int targetPoint) // вызов происходит по UI кнопке
        {
            PlayerPrefs.SetInt(PlayerPrefsStorage.PlayerCurrentPositionOnMap, targetPoint);
            isLerpMoving = true;
            yield return new WaitForSeconds(PlayerPrefsStorage.CoroutineForLerpDelay);
            isLerpMoving = false;
            print(PlayerPrefs.GetInt(PlayerPrefsStorage.PlayerCurrentPositionOnMap) + " ИЗ " + maxPointsCount);
            StartCoroutine(MoveIsCompleted()); // не запускается
        }
        
        private static IEnumerator MoveIsCompleted() 
        {
            yield return new WaitForSeconds(0.3f);
            print("Движение завершилось");
            //SceneManager.LoadScene("Main");
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