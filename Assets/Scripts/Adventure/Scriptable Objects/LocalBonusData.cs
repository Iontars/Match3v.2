#region Using
using UnityEngine;
#endregion

    namespace Adventure
    {
        /// <summary> Descriptions </summary>
        [CreateAssetMenu(fileName = "LocalBonus", menuName = "Game/Bonuses/LocalBonus")]
        public class LocalBonusData : ScriptableObject
        {
            public string bonusName;
            public int quantity;
        }
    }
