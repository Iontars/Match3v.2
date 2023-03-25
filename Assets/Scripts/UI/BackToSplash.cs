using Base_Game_Scripts;
using Game_Data_Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    /// <summary>
    /// Main/Board/EndGameManager
    /// </summary>
    public class BackToSplash : MonoBehaviour
    {
        public string sceneToLoad;
        GameData gameData;
        Board board;
        public void WinOK()
        {
            if (gameData != null)
            {
                gameData.saveData.isActive[board.Level + 1] = true;
                gameData.Save();
            }
            SceneManager.LoadScene(sceneToLoad);
        }
        public void LoseOK()
        {
            SceneManager.LoadScene(sceneToLoad);
        }

        private void Awake()
        {
            gameData = FindObjectOfType<GameData>();
            board = FindObjectOfType<Board>();
        }
        void Start()
        {
        
        }

        void Update()
        {
        
        }
    }
}
