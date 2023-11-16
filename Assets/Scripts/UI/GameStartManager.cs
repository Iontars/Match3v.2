#region Using

using UnityEngine;

#endregion

namespace UI
{
    /// <summary> Descriptions </summary>
    public class GameStartManager : MonoBehaviour
    {
        public GameObject startPanel;
        public GameObject levelPanel;

        private void Awake()
        {
            startPanel.SetActive(true);
            levelPanel.SetActive(false);
        }

        public void PlayGame()
        {
            startPanel.SetActive(false);
            levelPanel.SetActive(true);
        }

        public void Home()
        {
            startPanel.SetActive(true);
            levelPanel.SetActive(false);
        }
    }
}
