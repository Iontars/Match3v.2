#region Using

using Static_Prefs;
using UnityEngine;

#endregion

namespace Adventure.AdventureMap
{
    public class GlobalMapController : MonoBehaviour
    {
        public GameObject _playerOnMapGO;
        private PlayerOnMapPro _playerOnMapPro;
        public [] mapCheckPointsArray;
        public RollCube rollCube;
        private readonly int _adjustmentForEventField = 1;
        
        private void Awake()
        {
            _playerOnMapPro = _playerOnMapGO.GetComponent<PlayerOnMapPro>();
            _playerOnMapPro.currentPlayerMapPosition = new Vector2(
                mapCheckPointsArray[PlayerPrefs.GetInt(PlayerPrefsStorage.PlayerCurrentPositionOnMap)].transform.position.x,
                mapCheckPointsArray[PlayerPrefs.GetInt(PlayerPrefsStorage.PlayerCurrentPositionOnMap)].transform.position.y);
            _playerOnMapPro.maxPointsCount = mapCheckPointsArray.Length;
            Instantiate(_playerOnMapGO,  _playerOnMapPro.currentPlayerMapPosition, _playerOnMapGO.transform.rotation);
        }

        private void OnEnable()
        {
            rollCube.NumberReceived += TestMessage;
        }

        private void OnDisable()
        {
            rollCube.NumberReceived -= TestMessage;
        }

        private void TestMessage()
        {
            print(rollCube.RollNumber);
            CheckSpecialMapPosition();
            StartCoroutine(_playerOnMapPro.MoveStepBySteps(rollCube.RollNumber));
        }

        private void CheckSpecialMapPosition()
        {
            if (PlayerPrefs.GetInt(PlayerPrefsStorage.PlayerCurrentPositionOnMap) == 4)
            {
                PlayerPrefs.SetInt(PlayerPrefsStorage.PlayerCurrentPositionOnMap, 1 - _adjustmentForEventField);
            }
            if (PlayerPrefs.GetInt(PlayerPrefsStorage.PlayerCurrentPositionOnMap) == 7)
            {
                PlayerPrefs.SetInt(PlayerPrefsStorage.PlayerCurrentPositionOnMap, 4 - _adjustmentForEventField);
            }
        }
        void Start()
        {
        
        }


        void Update()
        {
       
        }
    }
}
