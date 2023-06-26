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

        private PlayerOnMap _playerOnMap;

        private void Awake()
        {
            _playerOnMap = FindObjectOfType<PlayerOnMap>();
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
            // PlayerPrefs.SetInt(PlayerPrefsStorage.PlayerCurrentPositionOnMap, int.Parse(gameObject.name) - 1);

            StartCoroutine(_playerOnMap.MoveTeleportToPoint(int.Parse(gameObject.name)));
            
            print(int.Parse(gameObject.name));
            
        }
    }
}
