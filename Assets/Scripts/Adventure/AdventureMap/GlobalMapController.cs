#region Using

using System;
using Static_Prefs;
using UnityEngine;

#endregion

namespace Adventure.AdventureMap
{
    public class GlobalMapController : MonoBehaviour
    {
        
        public GameObject _playerOnMapGO;
        private PlayerOnMapPro _playerOnMapPro;
        public GameObject[] mapCheckPointsArray;
        public RollCube rollCube;
        private readonly int _adjustmentForEventField = 1;
        
        private void Awake()
        {
            _playerOnMapPro = _playerOnMapGO.GetComponent<PlayerOnMapPro>();
            _playerOnMapPro.currentPlayerMapPosition =
                mapCheckPointsArray[PlayerPrefs.GetInt(PlayerPrefsStorage.PlayerCurrentPositionOnMap)].transform
                    .position;
            _playerOnMapPro.maxPointsCount = mapCheckPointsArray.Length;
            Instantiate(_playerOnMapGO,  _playerOnMapPro.currentPlayerMapPosition, _playerOnMapGO.transform.rotation);
        }

        private void OnEnable()
        {
            rollCube.NumberReceived += CallPlayerStepByStep;
            
            foreach (var item in mapCheckPointsArray)
            {
                item.GetComponent<ChechPoint_S>().action += CallPlayerTeleport;
            }
        }

        private void OnDisable()
        {
            // rollCube.NumberReceived -= CallPlayerStepByStep;
            // foreach (var item in mapCheckPointsArray)
            // {
            //     item.GetComponent<ChechPoint_S>().action -= CallPlayerTeleport; //ошибка
            // }
        }

        private void CallPlayerStepByStep()
        {
            print(rollCube.RollNumber);
            CheckSpecialMapPosition();
            StartCoroutine(_playerOnMapPro.MoveStepBySteps(rollCube.RollNumber));
        }
        
        private void CallPlayerTeleport(GameObject go)
        {
            if (go != null)
            {
                print("Problem");
                CheckSpecialMapPosition();
                StartCoroutine(_playerOnMapPro?.MoveTeleportToPoint(Array.IndexOf(mapCheckPointsArray, go)));
            }
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
