using System;
using Static_Prefs;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Adventure.AdventureMap
{
    public class RollCube : MonoBehaviour
    {
        public int RollNumber { get; private set; }
        private float _currentTime = 0;
        public Action<int> NumberReceived;
        public void GetRollNumber()
        {
            if (!(Time.time > _currentTime)) return;
            //RollNumber = Random.Range(1, 4);
            RollNumber = 2;
            print("Выброшено число: " + RollNumber);
            NumberReceived?.Invoke(RollNumber);
            _currentTime = Time.time + PlayerPrefsStorage.CoroutineForLerpDelay;

        }
    }
}
