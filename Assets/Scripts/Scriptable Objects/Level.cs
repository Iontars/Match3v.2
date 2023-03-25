using Base_Game_Scripts;
using UnityEngine;

namespace Scriptable_Objects
{
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
        public TileType[] boardLayout;

        [Header("Avalible dots")]
        public GameObject[] dots;

        [Header("Score Goal")]
        public int[] scoreGoals;

        [Header("End Game Requirements")]
        public EndGameRequirements endGameRequirements;

        public BlankGoal[] levelGoals;

    }
}
