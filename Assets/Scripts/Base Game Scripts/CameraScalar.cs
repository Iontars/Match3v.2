using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScalar : MonoBehaviour
{
    private Board board;
    public float cameraOffset;
    float aspectRatio;  // ширина разделённая на высоту разрешения экрана (1080/2340)
    public float pudding = 10;
    public float yOffset = 1;

    
    void Start()
    {
        aspectRatio = (float)Screen.width / Screen.height;
        //board = gameObject.AddComponent(typeof(Board)) as Board;
        board = FindObjectOfType<Board>();
        if (board != null)
        {
            RepositionCamera(board.Width - 1, board.Height - 1, cameraOffset);
        }
    }

    void RepositionCamera(float x, float y, float z)
    {
        Vector3 tempPosition = new Vector3(x/2, y/2 + yOffset, z);
        transform.position = tempPosition;
        if (board.Width >= board.Height)
        {
            //Camera.main.orthographicSize = board.height / 2 + pudding;
            Camera.main.orthographicSize = board.Height + 1;
            //Debug.LogWarning("ширина доски больше высоты");
        }
        else
        {
            //Camera.main.orthographicSize = (board.width / 2 + pudding) / aspectRatio;
            Camera.main.orthographicSize = board.Width + 1;
            //Debug.LogWarning("высота доски больше ширины");
        }
    }

    void Update()
    {
        
    }
}
