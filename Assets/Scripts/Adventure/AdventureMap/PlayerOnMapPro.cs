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
        public GlobalMapController globalMapController;
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

        private void OnEnable()
        {
        }

        private void MoveToPosition()
        {
            transform.position = Vector2.Lerp(transform.position,
                globalMapController.mapCheckPointsArray[PlayerPrefs.GetInt(PlayerPrefsStorage.PlayerCurrentPositionOnMap)].position,
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
            SceneManager.LoadScene("Main");
        }

        public IEnumerator MoveTeleportToPoint(int targetPoint)
        {
            PlayerPrefs.SetInt(PlayerPrefsStorage.PlayerCurrentPositionOnMap, targetPoint);
            isLerpMoving = true;
            yield return new WaitForSeconds(PlayerPrefsStorage.CoroutineForLerpDelay);
            isLerpMoving = false;
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