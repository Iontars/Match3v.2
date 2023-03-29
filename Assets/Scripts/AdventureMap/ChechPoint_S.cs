#region Using

using UnityEngine;

#endregion

namespace AdventureMap
{
    /// <summary> Descriptions </summary>
    public class ChechPoint_S : MonoBehaviour
    {
        private BoxCollider2D _collider;

        private void Start()
        {
            Debug.developerConsoleVisible = true;
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

    
    }
}
