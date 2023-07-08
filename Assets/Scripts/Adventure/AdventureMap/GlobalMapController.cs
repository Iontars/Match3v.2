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
        [HideInInspector] internal const int AdjustmentForEventField = 1;

        private void Awake()
        {
            _playerOnMapPro = _playerOnMapGO.GetComponent<PlayerOnMapPro>();
            _playerOnMapPro.currentPlayerMapPosition =
                mapCheckPointsArray[PlayerPrefs.GetInt(PlayerPrefsStorage.PlayerCurrentPositionOnMap)].transform
                    .position;
            _playerOnMapPro.maxPointsCount = mapCheckPointsArray.Length;
            Instantiate(_playerOnMapGO,  _playerOnMapPro.currentPlayerMapPosition, _playerOnMapGO.transform.rotation);
        }
    }
}
