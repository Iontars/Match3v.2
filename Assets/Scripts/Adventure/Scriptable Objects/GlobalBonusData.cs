#region Using
using UnityEngine;
#endregion

    namespace Adventure
    {
        /// <summary> Descriptions </summary>
        [CreateAssetMenu(fileName = "GlobalBonus", menuName = "Game/Bonuses/GlobalBonus")]
        public class GlobalBonusData : ScriptableObject
        {
            public string bonusName;
            public int quantity;
        }
    }
