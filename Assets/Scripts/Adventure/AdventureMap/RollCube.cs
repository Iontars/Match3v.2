using System;
using Static_Prefs;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Adventure.AdventureMap
{
    
    public class RollCube : MonoBehaviour
    {
        enum TurnEnum
        {
            Random,
            Two,
            Ten,
            One
        };

        [SerializeField] private TurnEnum turnEnum;
        
        public int RollNumber { get; private set; }
        private float _currentTime = 0;
        public Action<int> NumberReceived;
        public void GetRollNumber()
        {
            if (!(Time.time > _currentTime)) return;
            switch (turnEnum)
            {
                case TurnEnum.Random: RollNumber = Random.Range(1, 7); break;
                case TurnEnum.Ten: RollNumber = 10; break;
                case TurnEnum.Two: RollNumber = 2; break;
                case TurnEnum.One: RollNumber = 1; break;
            }
            
            print("Выброшено число: " + RollNumber);
            NumberReceived?.Invoke(RollNumber);
            _currentTime = Time.time + PlayerPrefsStorage.CoroutineForLerpDelay;

        }
    }
}
