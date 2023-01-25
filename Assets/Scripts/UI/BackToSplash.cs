using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
            gameData.saveData.isActive[board.level + 1] = true;
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
