using UnityEngine;
using UnityEngine.UI;

namespace Base_Game_Scripts
{
    public enum GameType // тип игнры на карте, на количество ходов или на время
    {
        Moves, Time
    }

    [System.Serializable]
    public class EndGameRequirements
    {
        public GameType gameType;
        public int counterValue;
    }

    /// <summary>
    /// Устанавливает условия победы или поражения
    /// </summary>
    public class EndGameManager : MonoBehaviour
    {
    
        public EndGameRequirements requirements;
        public GameObject timeLable;
        public GameObject movesLable;
        public GameObject youWinPanel;
        public GameObject tryAgainPanel;

        public Text counter;
        public int currentCounterValue; // счётчик ходов до окончания уровня

        float timerSeconds; // счётчик секунд до окончания уровня
        Board board;

        private void Awake()
        {
            board = FindObjectOfType<Board>();
        }

        // 1) установка параметров игры, загружаемых из SO
        public void SetGameType()
        {
            if (board.world != null)
            {
                if (board.Level < board.world.levels.Length)
                {
                    if (board.world.levels[board.Level] != null)
                    {
                        requirements = board.world.levels[board.Level].endGameRequirements;
                    } 
                }
            }
        }

        // настройка интерфейса игры исходя из выбранных праметров в 1)
        void SetupGame()
        {
            currentCounterValue = requirements.counterValue;
            if (requirements.gameType == GameType.Moves)
            {
                movesLable.SetActive(true);
                timeLable.SetActive(false);
            }
            else if (requirements.gameType == GameType.Time)
            {
                timerSeconds = 1;
                movesLable.SetActive(false);
                timeLable.SetActive(true);
            }
            counter.text = "" + currentCounterValue.ToString();
        }

        // уменьшение времени либо шагов необходиных ходов для победы
        public void DecreaseCountervalue()
        {
            if (board.currentState != GameState.Pause) // 1) Перенести эту обёртку  в более ранне место что бы сэкономит ресурсы, в метот Update в оператор IF requirements.gameType == GameType.Time && currentCounterValue > 0 && board.currentState != GameState.pause
            {
                currentCounterValue--;
                counter.text = "" + currentCounterValue.ToString();
                if (currentCounterValue <= 0)
                {
                    LoseGame();
                }
            }
        }

        // Настройка игры в режим победы, показ экрана победы, установка стейт машины на win, вызов анимации
        public void WinGame()
        {
            youWinPanel.SetActive(true);
            board.currentState = GameState.Win;
            print("isState " + board.currentState);
            counter.text = "" + (currentCounterValue = 0);
            FadePanelController fadePanelController = FindObjectOfType<FadePanelController>();
            fadePanelController.GameOver();
        }

        // Настройка игры в режим поражения, показ экрана поражения, установка стейт машины на lose, вызов анимации
        public void LoseGame()
        {
            // ?добавить ивент на поражение
            tryAgainPanel.SetActive(true);
            board.currentState = GameState.Lose;
            print("isState " + board.currentState);
            counter.text = "" + (currentCounterValue = 0);
            FadePanelController fadePanelController = FindObjectOfType<FadePanelController>();
            fadePanelController.GameOver();
        }

        void Start()
        {
            SetGameType();
            SetupGame();
        }
    
        void Update()
        {
            // если выбран режим игры на время но этот участ кода отвечает за это
            if (requirements.gameType == GameType.Time && currentCounterValue > 0) // 1)
            {
                timerSeconds -= Time.deltaTime;
                if (timerSeconds <= 0)
                {
                    //Debug.Log(" still active");
                    DecreaseCountervalue(); 
                    timerSeconds = 1;
                }
            }
        }
    }
}