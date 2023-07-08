#region Using

using UnityEngine;

#endregion

namespace Adventure.AdventureMap
{
    /// <summary> Descriptions </summary>
    public class GlobalBonusManager : MonoBehaviour
    {
        public AllBonusStorage allBonusStorage;
        private void Awake()
        {
            foreach (var item in allBonusStorage.globalBonusData)
            {
                print($"{item.bonusName} + {item.quantity}");
            }
        }

        private void Start()
        {
        
        }

        private void Update()
        {
        
        }
    }
}
