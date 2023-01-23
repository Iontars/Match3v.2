using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerPrefsStorage", menuName = "PLayerPrefsMenu")]
public class PlayerPrefsStorage : ScriptableObject
{
    [SerializeField]
    public static string keyCurrentLevel = "Current Level";
}
