using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScalar : MonoBehaviour
{
    private Board board;
    public float cameraOffset;
    //public float aspectRatio = 0.5625f; // ширина разделённая на высоту разрешения экрана (1080/1920)
    public float aspectRatio = .4615f; // ширина разделённая на высоту разрешения экрана (1080/1920)
    public float pudding = 10;
    //public float yOffset = 1;

    void Start()
    {
        //board = gameObject.AddComponent(typeof(Board)) as Board;
        board = FindObjectOfType<Board>();
        if (board != null)
        {
            RepositionCamera(board.width - 1, board.height - 1, cameraOffset);
        }
    }

    void RepositionCamera(float x, float y, float z)
    {
        Vector3 tempPosition = new Vector3(x/2, y/2, z);
        transform.position = tempPosition;
        if (board.width >= board.height)
        {
            Camera.main.orthographicSize = (board.width / 2 + pudding) / aspectRatio;
        }
        else
        {
            Camera.main.orthographicSize = board.height / 2 + pudding;
        }
    }

    void Update()
    {
        
    }
}
