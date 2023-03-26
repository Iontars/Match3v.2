using UnityEngine;

namespace Base_Game_Scripts
{
    public class CameraScalar : MonoBehaviour
    {
        private Board _board;
        private float _aspectRatio;  // ширина разделённая на высоту разрешения экрана (1080/2340)
        public float cameraOffset;
        public float pudding = 10;
        public float yOffset = 1;
        
        void Start()
        {
            _aspectRatio = (float)Screen.width / Screen.height;
            //board = gameObject.AddComponent(typeof(Board)) as Board;
            _board = FindObjectOfType<Board>();
            if (_board != null)
            {
                RepositionCamera(_board.Width - 1, _board.Height - 1, cameraOffset);
            }
        }

        void RepositionCamera(float x, float y, float z)
        {
            Vector3 tempPosition = new Vector3(x/2, y/2 + yOffset, z);
            transform.position = tempPosition;
            if (_board.Width >= _board.Height)
            {
                //Camera.main.orthographicSize = board.height / 2 + pudding;
                if (Camera.main != null) Camera.main.orthographicSize = _board.Height + 1;
                //Debug.LogWarning("ширина доски больше высоты");
            }
            else
            {
                //Camera.main.orthographicSize = (board.width / 2 + pudding) / aspectRatio;
                if (Camera.main != null) Camera.main.orthographicSize = _board.Width + 1;
                //Debug.LogWarning("высота доски больше ширины");
            }
        }
    }
}
