using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
/// <summary>
/// 
/// </summary>

[CreateAssetMenu (fileName = "World", menuName = "Level")]
public class Level : ScriptableObject
{
    [Header("Board Dimensions")]
    public int width;
    public int height;

    [Header("Start Tiles")]
    public TileKind[] boardLayout;

    [Header("Avalible dots")]
    public GameObject[] dots;

    [Header("Score Goal")]
    public int scoreGoals;

}
