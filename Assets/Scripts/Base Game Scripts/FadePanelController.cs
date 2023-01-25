using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// По нажатину на кнопку передаёт стейтмашине анимации, параметры
/// </summary>
public class FadePanelController : MonoBehaviour
{

    
    public Animator panelAnim;
    public Animator gameInfoAnim;

    public void OK()
    {
        panelAnim?.SetBool("Out", true);
        gameInfoAnim?.SetBool("Out", true);
        StartCoroutine(nameof(GameStartCo));
    }
       
    public void GameOver()
    {
        panelAnim?.SetBool("Out", false);
        panelAnim?.SetBool("Game Over", true);
    }

    //задержка после нажатия на OK что бы всё остальное встало на свои места
    IEnumerator GameStartCo()
    {
        yield return new WaitForSeconds(1.0f);
        Board board = FindObjectOfType<Board>();
        board.currentState = GameState.move;
    }
}
