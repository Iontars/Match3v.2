using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
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
