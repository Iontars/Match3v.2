using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerPrefsStorage", menuName = "PLayerPrefsMenu")]
public class PlayerPrefsStorage : ScriptableObject
{
    public static string pathToSaveFile = "/player.dat";

    [SerializeField]
    public static string keyCurrentLevel = "Current Level";
}
