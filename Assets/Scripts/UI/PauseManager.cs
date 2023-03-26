#region Using

using Base_Game_Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#endregion


namespace UI
{
    /// <summary> Descriptions </summary>
    public class PauseManager : MonoBehaviour
    {
        public GameObject pausePanel;
        private Board _board;
        public bool paused = false;
        public Image soundButton;
        public Sprite musicOnSprite;
        public Sprite musicOffSprite;
        private SoundManager _sound; // убрать здесь ему не место
        private void Awake()
        {
            _sound = FindObjectOfType<SoundManager>();
            _board = GameObject.FindWithTag("Board").GetComponent<Board>();
        }

        private void Start()
        {
            // в PlayerPrefs ключ "Sound" для звука, 0 - mute, 1 - unmute
            pausePanel.SetActive(false);
            if (PlayerPrefs.HasKey("Sound"))
            {
                soundButton.sprite = PlayerPrefs.GetInt("Sound") == 0 ? musicOffSprite : musicOnSprite;
            }
            else
            {
                soundButton.sprite = musicOnSprite;
            }
        }

        public void PauseGame()
        {
            paused = !paused;
        }

        public void ExitGame()
        {
            SceneManager.LoadScene("Splash");
        }

        public void SoundButton()
        {
            if (PlayerPrefs.HasKey("Sound"))
            {
                if (PlayerPrefs.GetInt("Sound") == 0)
                {
                    soundButton.sprite = musicOnSprite;
                    PlayerPrefs.SetInt("Sound", 1);
                    _sound.AdjustValue();
                }
                else
                {
                    soundButton.sprite = musicOffSprite;
                    PlayerPrefs.SetInt("Sound", 0);
                    _sound.AdjustValue();
                }
            }
            else
            {
                soundButton.sprite = musicOffSprite;
                PlayerPrefs.SetInt("Sound", 1);
                _sound.AdjustValue();
            }
        }

        private void Update()
        {
            if (paused && !pausePanel.activeInHierarchy)
            {
                pausePanel.SetActive(true);
                _board.currentState = GameState.Pause;
            }
            if (!paused && pausePanel.activeInHierarchy)
            {
                pausePanel.SetActive(false);
                _board.currentState = GameState.Move;
            }   
        }
    }
}
