using System;
using UnityEngine;
using System.IO;

namespace Static_Prefs
{
    public class BonusGlobal : MonoBehaviour
    {
        public string jsonGlobalBonusPath;
        public string jsonGlobalBonusText;

        private void Awake()
        {
            jsonGlobalBonusPath = "Assets/Resources/JSON/Bonuses/GlobalBonuses.json";
            if (File.Exists(jsonGlobalBonusPath))
            {
                jsonGlobalBonusText = File.ReadAllText(jsonGlobalBonusPath);
            }
            else
            {
                print("Application.Quit");
                Application.Quit();
            }
        }
        void Start()
        {
        
        }
    
        void Update()
        {
        
        }
    }
}
