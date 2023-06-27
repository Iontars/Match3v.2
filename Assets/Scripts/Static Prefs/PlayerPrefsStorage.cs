using UnityEngine;

namespace Static_Prefs
{
    [CreateAssetMenu(fileName = "PlayerPrefsStorage", menuName = "PLayerPrefsMenu")]
    public class PlayerPrefsStorage : ScriptableObject
    {
        public const string PathToSaveFile = "/player.dat";
        public const string KeyCurrentLevel = "Current Level";
        public const string TagBoard = "Board";
        public const string PlayerCurrentPositionOnMap = "PlayerCurrentPositionOnMap";
        public const string NumberOfCurrentLevel = "NumberOfCurrentLevel";
        
        public const float CoroutineForLerpDelay = 1.2f;

    }
}
