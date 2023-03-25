using UnityEngine;

namespace Scriptable_Objects
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu (fileName = "World", menuName = "World")]
    public class World : ScriptableObject
    {
        public Level[] levels;

        void Awake()
        {
        
        }

    }
}
