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
    public void OK()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
