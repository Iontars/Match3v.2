#region Using

using System;
using Adventure.AdventureMap;
using Static_Prefs;
using UnityEngine;

#endregion

namespace AdventureMap
{
    /// <summary> Descriptions </summary>
    public class ChechPoint_S : MonoBehaviour
    {
        private BoxCollider2D _collider;

        private PlayerOnMapPro _playerOnMap;
        private GlobalMapController _globalMapController;

        //public Action action;

        private void Awake()
        {
            _playerOnMap = FindObjectOfType<PlayerOnMapPro>();
            _globalMapController = FindObjectOfType<GlobalMapController>();
        }

        private void Start()
        {
            _collider = GetComponent<BoxCollider2D>();
        }

        private void OnDrawGizmos()
        {
            if (_collider != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(_collider.bounds.center, _collider.bounds.size);
            }
        }

        private void OnMouseDown()
        {
            //action.Invoke();
            print(Array.IndexOf(_globalMapController.mapCheckPointsArray, gameObject.transform.position));
            StartCoroutine(_playerOnMap.MoveTeleportToPoint(Array.IndexOf(_globalMapController.mapCheckPointsArray, gameObject)));

        }
    }
}
