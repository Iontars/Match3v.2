#region Using

using System;
using UnityEngine;

#endregion

namespace Adventure.AdventureMap
{
    /// <summary> Descriptions </summary>
    public class CheckPoint_S : MonoBehaviour
    {
        public int number;
        private BoxCollider2D _collider;

        private PlayerOnMapPro _playerOnMap;
        private GlobalMapController _globalMapController;

        public Action<int> Action;

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
            Action?.Invoke(number);
        }
    }
}
