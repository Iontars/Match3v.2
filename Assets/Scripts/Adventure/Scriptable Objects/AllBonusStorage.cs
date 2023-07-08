#region Using
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using Adventure;
using TMPro;
#endregion

    /// <summary> Descriptions </summary>
    [CreateAssetMenu(fileName = "BonusStorage", menuName = "Game/Bonuses/AllBonusStorage")]
    public class AllBonusStorage : ScriptableObject
    {
        public GlobalBonusData[] globalBonusData;
        public LocalBonusData[] localBonusData;
    }
