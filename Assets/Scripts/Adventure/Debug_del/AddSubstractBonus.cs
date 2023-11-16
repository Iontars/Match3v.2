#region Using
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using Adventure.AdventureMap;
using TMPro;
#endregion

    /// <summary> Descriptions </summary>
public class AddSubstractBonus : MonoBehaviour 
{ 
    public GlobalBonusManager globalBonusManager;
    
    public void AddBonus_1()
    {
        globalBonusManager.allBonusStorage.globalBonusData[0].quantity++;
    }
    
    public void SubBonus_1()
    {
        globalBonusManager.allBonusStorage.globalBonusData[0].quantity--;
    }
    
    public void AddBonus_2()
    {
        
    }
    
    public void AddBonus_3()
    {
        
    }
    
    public void AddBonus_4()
    {
        
    }
    
    public void AddBonus_5()
    {
        
    }
    
    public void AddBonus_6()
    {
        
    }
}
