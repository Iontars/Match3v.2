using System.Collections;
using System.Collections.Generic;
using Scriptable_Objects;
using Static_Prefs;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Base_Game_Scripts
{
    public enum GameState{Wait, Move, Win, Lose, Pause} // состояние используется для блокировки повторного свайпа пока токены движутся и меняет состояние игры

    public enum TileKind {Breakable, Blank, Normal, Lock, Concrete, Slime } // состояние тайлов, говорящще о том занял ли тайл или нет

    [System.Serializable]
    public class TileType // класс хранящйи в себе информацию о том занят ли тайл или нет
    {
        public int x;
        public int y;
        public TileKind tileKind;
    }

    [System.Serializable]
    public class MatchType
    {
        public int type;
        public string color;
    }

    /// <summary>
    /// Постороение игрового поля, создание токенов, поиск различных совпадений, уничтожение токенов, звук токенов,
    /// добавление очков за токены, распознавание пустых мест а так же границы доски, перезаполнение доски.
    /// </summary>
    public class Board : MonoBehaviour
    {
        public World world;
        public int Level { get; private set; }
        public GameState currentState = GameState.Move; // ?Рудимент

        private readonly int _offSet = 2; // смещает позициаю спавна точек по оси х что позволяет им скользить вниз при появлении
        public int Width { get; private set; }
        public int Height { get; private set; }

        [Header("Prefabs")]
        private GameObject[] _currentLevelUsableTokens; // какие игровые токены будут использоваться на текущей доске
        [SerializeField] public GameObject padTilePrefab; // подложка под токена на доске
        [SerializeField] public GameObject breakableTilePrefab;
        [SerializeField] public GameObject lockTilePrefab;
        [SerializeField] public GameObject concreteTilePrefab;
        [SerializeField] public GameObject slimeTilePrefab;
        [SerializeField] public GameObject destroyEffect;
        
        [Header("Layout")]
        private bool[,] _blankSpaces; // массив соразмерный текущей доске для хранения значений о зарезервированных местах
        private BackgroundTile[,] _breakableTiles; // массив с ломающимися плитками на доске содержит компоненты
        private BackgroundTile[,] _concreteTiles;
        private BackgroundTile[,] _slimeTiles;
        public BackgroundTile[,] lockTiles; // publick becouse need accces from dataScript
        public TileType[] boardLayout;
        public GameObject[,] currentLevelAllTokensArray; // массив со всеми игровыми точками на доске

        [Header("Match Stuff")]
        private FindMatches _findMatches;
        private SoundManager _soundManager;
        private ScoreManager _scoreManager;
        private GoalManager _goalManager; // vid 40
        private int _streakValue = 1;
        private bool _makeSlime = true;
        private readonly float _refillDelay = 0.5f;
        public Dot currentDot; // запись в скрипте Dot метод CalculateAngle
        public MatchType matchType;
        public int basePieceValue = 5;
        public int[] scoreGoals; // сколько нужно набрать очков для различных успехов на карте, 1 звезда 2000 очков 2 звезды 4000 очков итд

        private void Awake()
        {
            padTilePrefab = Resources.Load<GameObject>("Prefabs/Tile Background");
            breakableTilePrefab = Resources.Load<GameObject>("Prefabs/Breakable Tile");
            lockTilePrefab = Resources.Load<GameObject>("Prefabs/Lock Tile");
            concreteTilePrefab = Resources.Load<GameObject>("Prefabs/Concrete Tile");
            slimeTilePrefab = Resources.Load<GameObject>("Prefabs/Slime Tile");
            destroyEffect = Resources.Load<GameObject>("Prefabs/Destroy Effect");
            
            _scoreManager = FindObjectOfType<ScoreManager>();
            _goalManager = FindObjectOfType<GoalManager>();
            _soundManager = FindObjectOfType<SoundManager>();
            LoadLevelByHisNumber();
            _findMatches = FindObjectOfType<FindMatches>();
            _breakableTiles = new BackgroundTile[Width, Height];
            lockTiles = new BackgroundTile[Width, Height];
            _concreteTiles = new BackgroundTile[Width, Height];
            _slimeTiles = new BackgroundTile[Width, Height];
            currentLevelAllTokensArray = new GameObject[Width, Height];
        }

        private void Start()
        {
            currentState = GameState.Pause; // игра начинается с состояния паузы !!!!!!!!!!!!
            LoadLevelByHisNumber();
            SetUp();
        }

        private void LoadLevelByHisNumber()
        {
            //загрузка номера уровня переданного из сцены выбора уровней
            if (PlayerPrefs.HasKey(PlayerPrefsStorage.KeyCurrentLevel))
            {
                Level = PlayerPrefs.GetInt(PlayerPrefsStorage.KeyCurrentLevel);
            }
            
            if (world != null && Level < world.levels.Length && world.levels[Level] != null )
            {
                //копирование значений из выбранного уровня в нашу доску
                Width = world.levels[Level].width;
                Height = world.levels[Level].height;
                _currentLevelUsableTokens = world.levels[Level].dots;
                scoreGoals = world.levels[Level].scoreGoals;
                boardLayout = world.levels[Level].boardLayout;
                _blankSpaces = new bool[Width, Height];
            }
        }

        private void GenerateReservesPlaceToken()
        {
            foreach (var t in boardLayout)
            {
                var tempPosition = new Vector2(t.x, t.y);

                if (t.tileKind == TileKind.Blank)
                {
                    _blankSpaces[t.x, t.y] = true;
                }
                else if (t.tileKind == TileKind.Slime)
                {
                    GameObject reserveToken = Instantiate(slimeTilePrefab, tempPosition, Quaternion.identity);
                    _slimeTiles[t.x, t.y] = reserveToken.GetComponent<BackgroundTile>();
                }
                else if(t.tileKind == TileKind.Breakable)
                {
                    GameObject reserveToken = Instantiate(breakableTilePrefab, tempPosition, Quaternion.identity);
                    _breakableTiles[t.x, t.y] = reserveToken.GetComponent<BackgroundTile>();
                }
                else if(t.tileKind == TileKind.Lock)
                {
                    GameObject reserveToken = Instantiate(lockTilePrefab, tempPosition, Quaternion.identity);
                    lockTiles[t.x, t.y] = reserveToken.GetComponent<BackgroundTile>();
                }
                else if(t.tileKind == TileKind.Concrete)
                {
                    GameObject reserveToken = Instantiate(concreteTilePrefab, tempPosition, Quaternion.identity);
                    _concreteTiles[t.x, t.y] = reserveToken.GetComponent<BackgroundTile>();
                }
            }
        }
        
        //проверка на одинаковые токены перед создании доски
        private bool CheckForSameTokensAround(int column, int row, GameObject checkableToken)
        {
            if (column > 1 && row > 1 )
            {
                if (currentLevelAllTokensArray[column -1, row] != null && currentLevelAllTokensArray[column - 2, row] != null) //проверка относящаяся к зарезервированым местам на доске
                {
                    if (currentLevelAllTokensArray[column - 1, row].CompareTag(checkableToken.tag) &&
                        currentLevelAllTokensArray[column - 2, row].CompareTag(checkableToken.tag))
                    {
                        return true;
                    }
                }
                if (currentLevelAllTokensArray[column, row - 1] != null && currentLevelAllTokensArray[column, row - 2] != null) //проверка относящаяся к зарезервированым местам на доске
                {
                    if (currentLevelAllTokensArray[column, row - 1].CompareTag(checkableToken.tag) &&
                        currentLevelAllTokensArray[column, row - 2].CompareTag(checkableToken.tag))
                    {
                        return true;
                    }
                }
            }
            else if(column <= 1 || row <= 1)
            {
                if (row > 1)
                {
                    if (currentLevelAllTokensArray[column, row - 1] != null && currentLevelAllTokensArray[column, row - 2] != null) //проверка относящаяся к зарезервированым местам на доске
                    {
                        if (currentLevelAllTokensArray[column, row - 1].CompareTag(checkableToken.tag) &&
                            currentLevelAllTokensArray[column, row - 2].CompareTag(checkableToken.tag))
                        {
                            return true;
                        }
                    }
                }
                if (column > 1) //
                {
                    if (currentLevelAllTokensArray[column - 1, row] != null && currentLevelAllTokensArray[column - 2, row] != null) //проверка относящаяся к зарезервированым местам на доске
                    {
                        if (currentLevelAllTokensArray[column - 1, row].CompareTag(checkableToken.tag) &&
                            currentLevelAllTokensArray[column - 2, row].CompareTag(checkableToken.tag))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        //создание доски
        private void SetUp()
        {
            GenerateReservesPlaceToken(); // перед созданием тайлов на доске, создаём зарезервированные места
            for (var i = 0; i < Width; i++)
            {
                for (var j = 0; j < Height; j++)
                {
                    // подложки спавним только не на пустых зарезевриванных местах
                    if (!_blankSpaces[i, j]) 
                    {
                        Vector2 tilePosition = new Vector2(i, j);
                        GameObject backgroundTile = Instantiate(padTilePrefab, tilePosition, Quaternion.identity) as GameObject;
                        backgroundTile.transform.parent = transform;
                        backgroundTile.name = "( " + i + ", " + j + " )";
                    }
                    
                    // обычные токены спавним на всех свободныех от резервации местах
                    if (!_blankSpaces[i, j] && !_concreteTiles[i,j] && !_slimeTiles[i,j]) // проверка на пустое место/бетонное место на доске/если false создаёт тайл на доске
                    {
                        var checkableTokenValue = Random.Range(0, _currentLevelUsableTokens.Length);
                        //вызов проверки на совпадение при создании доски (не должно быть готовых совпадений)
                        int maxIterations = 0;
                        while (CheckForSameTokensAround(i, j, _currentLevelUsableTokens[checkableTokenValue]) && maxIterations < 100)
                        {
                            checkableTokenValue = Random.Range(0, _currentLevelUsableTokens.Length);
                            maxIterations++;
                            //Debug.Log(maxIterations);
                        }
                        maxIterations = 0;

                        Vector2 tempPosition = new Vector2(i, j);
                        GameObject createdToken = Instantiate(_currentLevelUsableTokens[checkableTokenValue], tempPosition, Quaternion.identity);
                        createdToken.GetComponent<Dot>().row = j;
                        createdToken.GetComponent<Dot>().column = i;
                        createdToken.transform.parent = transform;
                        createdToken.name = "( " + i + ", " + j + " )";
                        currentLevelAllTokensArray[i, j] = createdToken;
                    }
                }
            }
        }
        
        private MatchType ColumnOrRow()
        {
            // создать копию FindMatches.currentMatches
            List<GameObject> matchCopy = _findMatches.currentMatches; // as Важно изучить

            matchType.type = 0;
            matchType.color = "";

            // Просмотреть список и решить нужно ли делать бомбу
            for (int i = 0; i < matchCopy.Count; i++)
            {
                // сохраним текущую точку
                Dot thisDot = matchCopy[i].GetComponent<Dot>();
                string color = matchCopy[i].tag;
                //int column = thisDot.column;
                //int row = thisDot.row;
                int columnMatch = default;
                int rowMatch = default;
                // просмотреть остальные соседние точки и сравнить
                for (int j = 0; j < matchCopy.Count; j++)
                {
                    Dot nextDot = matchCopy[j]?.GetComponent<Dot>();
                    if (thisDot == nextDot)
                    {
                        continue;
                    }
                    if (thisDot.column == nextDot.column && nextDot.CompareTag(color)) // перейти на компонентную систему заменив теги
                    {
                        columnMatch++;
                    }
                    if (thisDot.row == nextDot.row && nextDot.CompareTag(color)) // перейти на компонентную систему заменив теги
                    {
                        rowMatch++;
                    }
                }
                //return 1 если цветная бомба
                //return 2 если бомба уничтожающая соседние тайлы
                //return 3 если колонки или строки совпали
                if (columnMatch == 4 || rowMatch == 4) //  сравниваем с числом меньше фактического так как не учитываем сами себя
                {
                    matchType.type = 1;
                    matchType.color = color;
                    return matchType;
                }
                if (columnMatch == 2 && rowMatch == 2)
                {
                    matchType.type = 2;
                    matchType.color = color;
                    return matchType;
                }
                if (columnMatch == 3 || rowMatch == 3)
                {
                    matchType.type = 3;
                    matchType.color = color;
                    return matchType;
                }          
            }
            matchType.type = 0;
            matchType.color = "";
            return matchType;
        }


        // Vid 51.2 (16 min) как то связано с уничтожением бетона если его задеват горизонтальная бомба
        public void BombRow(int row)
        {
            for (int i = 0; i < Width; i++)
            {
            
                if (_concreteTiles[i, row])
                {
                    _concreteTiles[i, row].TakeDamage(1);
                    if (_concreteTiles[i, row].hitPoints <= 0)
                    {
                        _concreteTiles[i, row] = null; // удаляем из массива ломающийся токен
                    }
                }
            }
        }

        public void BombColumn(int column)
        {
            for (int i = 0; i < Width; i++)
            {
                if (_concreteTiles[column, i])
                {
                    _concreteTiles[column, i].TakeDamage(1);
                    if (_concreteTiles[column, i].hitPoints <= 0)
                    {
                        _concreteTiles[column, i] = null; // удаляем из массива ломающийся токен
                    }
                }
            }
        }

        // Уничтожение совпавших токенов // тут же подсчёт очков // звук ломания токена выделить в отдельный ивент/метод/ партикл взрыва / урон бонусным тайлам
        void DestroyMatchesAt(int colunm, int row)
        {
            if (currentLevelAllTokensArray[colunm,row].GetComponent<Dot>().isMatched)
            {
                // сколько эллементов в Списке currentMatches ?
                if (_findMatches.currentMatches.Count >=4)
                {
                    CheckToMakeBombs();
                }

                //эти уничтожения работают потому что они находятся на одной позиции с токеном, с бетоной стеной ест ьотдельный класс DamageConcrete
                // нужно ли разбивать ломающуюся плитку
                if (_breakableTiles[colunm, row] != null) //  если в данной позиции есть ломающаяся плитка
                {
                    // если это нанесёт 1 единицу урона
                    _breakableTiles[colunm, row].TakeDamage(1);
                    if (_breakableTiles[colunm, row].hitPoints <= 0)
                    {
                        _breakableTiles[colunm, row] = null; // удаляем из массива ломающийся токен
                    }
                }

                if (lockTiles[colunm, row] != null)
                {
                    // если это нанесёт 1 единицу урона
                    lockTiles[colunm, row].TakeDamage(1);
                    if (lockTiles[colunm, row].hitPoints <= 0)
                    {
                        lockTiles[colunm, row] = null; // удаляем из массива ломающийся токен
                    }
                }

                DamageConcrete(colunm, row); // урон по соседним от isMatched бомбам
                DamageSlime(colunm, row); // урон по соседним от isMatched слаймам
                CheckToMakeSlime();
            
            
                // vid 40 // сравниваем ломающуюся плитку на предмет цели уровня
                if (_goalManager != null)
                {
                    _goalManager.CompareGoal(currentLevelAllTokensArray[colunm, row].tag.ToString());
                    _goalManager.UpdateGoals();
                }

                _soundManager.PlayRandomDestroyNoise(); // звук ломания токена

                GameObject particle = Instantiate(destroyEffect, new Vector3( // анимация взрыва партиклом
                    currentLevelAllTokensArray[colunm, row].transform.position.x,
                    currentLevelAllTokensArray[colunm, row].transform.position.y,
                    currentLevelAllTokensArray[colunm, row].transform.position.z - 1), Quaternion.identity);
                Destroy(particle, .3f);
                currentLevelAllTokensArray[colunm,row].GetComponent<Dot>().PopAnimation(); // анимация спрайта
                Destroy(currentLevelAllTokensArray[colunm, row], .2f); // Фактическое уничтожение совпавших бомб
                _scoreManager.IncreaseScore(basePieceValue * _streakValue); // добавление очков на табло
                currentLevelAllTokensArray[colunm, row] = null;
            
            }
        }

        // вызов Уничтожение совпавших токенов
        public void DestroyMatches() // зачем так сложно в 500 обёрток // оптимизировать. объеденить с методом DestroyMatchesAt
        {
            // сколько эллементов в Списке currentMatches ?
            // как только мы проверили что в списке совпадений существует 4 или более совпадений мы сразу проверяем какую бомбу можно создать и 
            // очищаем список совпадений в избежании повторных сравнений в процессе больших каскадов и больших цепочек совпадений
            if (_findMatches.currentMatches.Count >= 4)
            {
                CheckToMakeBombs();
            }
            _findMatches.currentMatches.Clear();
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if (currentLevelAllTokensArray[i,j] != null)
                    {
                        DestroyMatchesAt(i, j);
                    }
                }
            }
            _findMatches.currentMatches.Clear();
            StartCoroutine(nameof(DecreaseRowCo2));
        }

        // проверить какой тип бонуса создать при совпадениях
        private void CheckToMakeBombs()
        {
            // Скольк осовпадений за раз находится в списке FindMatches.currentMatches
            if (_findMatches.currentMatches.Count > 3)
            {
                // какой тип срвпадений?
                MatchType typeOfMatch = ColumnOrRow();
                if (typeOfMatch.type == 1)
                {
                    //Make a color bomb
                    //is the current dot matched?
                    if (currentDot != null && currentDot.isMatched && currentDot.tag == typeOfMatch.color)
                    {
                        currentDot.isMatched = false;
                        currentDot.MakeColorBomb();
                    }
                    else
                    {
                        if (currentDot.otherDot != null)
                        {
                            Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                            if (otherDot.isMatched && otherDot.tag == typeOfMatch.color)
                            {
                                otherDot.isMatched = false;
                                otherDot.MakeColorBomb();
                            }
                        }
                    }
                }
                else if (typeOfMatch.type == 2)
                {
                    //Make a adjacent bomb
                    //is the current dot matched?
                    if (currentDot != null && currentDot.isMatched && currentDot.tag == typeOfMatch.color)
                    {
                        currentDot.isMatched = false;
                        currentDot.MakeAjacentBomb();
                    }
                    else if (currentDot.otherDot != null)
                    {
                        Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                        if (otherDot.isMatched && otherDot.tag == typeOfMatch.color)
                        {
                            otherDot.isMatched = false;
                            otherDot.MakeAjacentBomb();
                        }
                    }
                }
                else if (typeOfMatch.type == 3)
                {
                    _findMatches.CheckBombs(typeOfMatch); // если в совпадениях 3 токена то смотрим только лишь на линейные бомбы
                }
            }
        }

        // урон по соседним бетонным стенам
        private void DamageConcrete(int column, int row)
        {
            // проверка есть ли бетонная стена слева от токена, смотрим только позиции отличные от нуля так как левее границы поля не может быть бетонной стены
            if (column > 0)
            {
                if (_concreteTiles[column - 1, row] != null)
                {
                    _concreteTiles[column - 1, row].TakeDamage(1);
                    if (_concreteTiles[column - 1, row].hitPoints <= 0)
                    {
                        _concreteTiles[column - 1, row] = null; // удаляем из массива ломающийся токен
                    }
                }
            }
            // проверка есть ли бетонная стена справа от токена
            if (column < Width - 1)
            {
                if (_concreteTiles[column + 1, row] != null)
                {
                    _concreteTiles[column + 1, row].TakeDamage(1);
                    if (_concreteTiles[column + 1, row].hitPoints <= 0)
                    {
                        _concreteTiles[column + 1, row] = null; // удаляем из массива ломающийся токен
                    }
                }
            }
            if (row > 0)
            {
                if (_concreteTiles[column, row - 1] != null)
                {
                    _concreteTiles[column, row - 1].TakeDamage(1);
                    if (_concreteTiles[column, row - 1].hitPoints <= 0)
                    {
                        _concreteTiles[column, row - 1] = null; // удаляем из массива ломающийся токен
                    }
                }
            }
            if (row < Height - 1)
            {
                if (_concreteTiles[column, row + 1] != null)
                {
                    _concreteTiles[column, row + 1].TakeDamage(1);
                    if (_concreteTiles[column, row + 1].hitPoints <= 0)
                    {
                        _concreteTiles[column, row + 1] = null; // удаляем из массива ломающийся токен
                    }
                }
            }
        }
    
        // урон по соседним слайм стенам
        private void DamageSlime(int column, int row)
        {
            // проверка есть ли бетонная стена слева от токена, смотрим только позиции отличные от нуля так как левее границы поля не может быть бетонной стены
            if (column > 0)
            {
                if (_slimeTiles[column - 1, row] != null)
                {
                    _slimeTiles[column - 1, row].TakeDamage(1);
                    if (_slimeTiles[column - 1, row].hitPoints <= 0)
                    {
                        _slimeTiles[column - 1, row] = null; // удаляем из массива ломающийся токен
                    }
                    _makeSlime = false;
                }
            }
            // проверка есть ли бетонная стена справа от токена
            if (column < Width - 1)
            {
                if (_slimeTiles[column + 1, row] != null)
                {
                    _slimeTiles[column + 1, row].TakeDamage(1);
                    if (_slimeTiles[column + 1, row].hitPoints <= 0)
                    {
                        _slimeTiles[column + 1, row] = null; // удаляем из массива ломающийся токен
                    }
                    _makeSlime = false;
                }
            }
            if (row > 0)
            {
                if (_slimeTiles[column, row - 1] != null)
                {
                    _slimeTiles[column, row - 1].TakeDamage(1);
                    if (_slimeTiles[column, row - 1].hitPoints <= 0)
                    {
                        _slimeTiles[column, row - 1] = null; // удаляем из массива ломающийся токен
                    }
                    _makeSlime = false;
                }
            }
            if (row < Height - 1)
            {
                if (_slimeTiles[column, row + 1] != null)
                {
                    _slimeTiles[column, row + 1].TakeDamage(1);
                    if (_slimeTiles[column, row + 1].hitPoints <= 0)
                    {
                        _slimeTiles[column, row + 1] = null; // удаляем из массива ломающийся токен
                    }
                    _makeSlime = false;
                }
            }
        }

        // продвинутое распознавание пустых мест на доске
        private IEnumerator DecreaseRowCo2()
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    // не зарезервировано ли место внутри доски? работает в совокупности с методом RefillBoard() и таким же условием в нем
                    if (!_blankSpaces[i,j] && currentLevelAllTokensArray[i,j] == null && !_concreteTiles[i, j] && !_slimeTiles[i,j])
                    {
                        // проверка доски
                        for (int k = j + 1; k < Height; k++)
                        {
                            // найдена ли точка ?
                            if (currentLevelAllTokensArray[i,k] != null)
                            {
                                // переместить точки в пустое пространство
                                currentLevelAllTokensArray[i, k].GetComponent<Dot>().row = j;
                                // установить значение этого места как null
                                currentLevelAllTokensArray[i, k] = null;
                                break;
                            }
                        }
                    }
                }
            }
            yield return new WaitForSeconds(_refillDelay * 0.5f); // не трогать, задержка перед появлением новых токенов
            StartCoroutine(nameof(FillBoardCo));      
        }

        private IEnumerator FillBoardCo()
        {
            yield return new WaitForSeconds(_refillDelay);
            RefillBoard();
            yield return new WaitForSeconds(_refillDelay);
            while (MatchesOnBoard())
            {
                _streakValue++;
                DestroyMatches();
                yield break;
            }
            currentDot = null;
            CheckToMakeSlime();
            if (IsDeadlocked())
            {
                StartCoroutine(nameof(ShuffleBoard));
            }
            yield return new WaitForSeconds(_refillDelay);
            Debug.Log("Done Refilling");
            System.GC.Collect();
            if (currentState != GameState.Pause) // vid 55 (9 min)
                currentState = GameState.Move;
            _makeSlime = true;
            _streakValue = 1;

            // RefillBoard(); // заполнить досу токенами после первого совпадения
            // yield return new WaitForSeconds(refillDelay); // и подождать
            // // каскад совпадений
            // while (MatchesOnBoard()) // пока есть каскад совпадений, ждать wait
            // {
            //     streakValue++; // серия совпадений
            //     //currentState = GameState.wait;
            //     print(streakValue);
            //     DestroyMatches(); // вызов этого метода должен быть раньше задержки
            //     //yield return new WaitForSeconds(1.5f * refillDelay); // нужно дождаться заполнения доски прежде чем проверить поэтому увеличиваем время
            //     yield break;
            // }
            // findMatches.currentMatches.Clear(); // Имеет отношенеи к бонусам
            // CheckToMakeSlime();
            // yield return new WaitForSeconds(refillDelay);
            //
            //
            // if (IsDeadlocked()) // проверка на наличие того что на доске больше нельзя создать совпадений
            // {
            //     ShuffleBorad();
            // }
            // if (!MatchesOnBoard())
            // {
            //     currentState = GameState.move;
            //     _makeSlime = true;
            //     streakValue = 1;
            // }
        }

        #region Slime

        private void CheckToMakeSlime()
        {
            // проверить массив слаймов
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if (_slimeTiles[i,j] != null && _makeSlime)
                    {
                        // вызвать другой метод для создания нового слайма
                        Debug.Log("Slime");
                        MakeNewSlime();
                        return;
                    }
                }
            }
        }

        //проверка на соседнюю бомбу vid 53 (19) связано со слаймами
        private Vector2 CheckForAdjacent(int column, int row)
        {

            if (column < Width - 1 && currentLevelAllTokensArray[column + 1, row])
            {
                return Vector2.right;
            }
            if (column > 0 && currentLevelAllTokensArray[column - 1, row])
            {
                return Vector2.left;
            }
            if (row < Height - 1 && currentLevelAllTokensArray[column, row + 1])
            {
                return Vector2.up;
            }
            if (row > 0 && currentLevelAllTokensArray[column, row - 1])
            {
                return Vector2.down;
            }
            return Vector2.zero;
        }

        private void MakeNewSlime() // vid 53 (27 min)
        {
            bool slime = false;
            int loops = 0; //  что бы не попасть в вечную проверку
            while (!slime && loops < 200)
            {
                int newX = Random.Range(0, Width);
                int newY = Random.Range(0, Height);
                if (_slimeTiles[newX, newY]) // проверяем есть ли по даному индексу массива плитка слайма
                {
                    Vector2 adjacent = CheckForAdjacent(newX, newY);
                    if (adjacent != Vector2.zero)
                    {
                        Destroy(currentLevelAllTokensArray[newX + (int)adjacent.x, newY + (int)adjacent.y]);
                        Vector2 tempPosition = new Vector2(newX + (int)adjacent.x, newY + (int)adjacent.y);
                        GameObject tile = Instantiate(slimeTilePrefab, tempPosition, Quaternion.identity);
                        _slimeTiles[newX + (int)adjacent.x, newY + (int)adjacent.y] = tile.GetComponent<BackgroundTile>();
                        slime = true;
                    }
                }
                loops++;
            }
        }
        #endregion

        // заполнение пустых ячеек
        private void RefillBoard()
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if (currentLevelAllTokensArray[i,j] == null && !_blankSpaces[i,j] && !_concreteTiles[i, j] && !_slimeTiles[i, j]) // проверка в том числе на зарезервированные места на доске
                    {
                        Vector2 tempPosition = new Vector2(i, j + _offSet);
                        int dotToUse = Random.Range(0, _currentLevelUsableTokens.Length); 

                        //фикс проблемы когда при каскаде можно было передвиграть фгуры вручную
                        int maxIterations = 0;
                        while (CheckForSameTokensAround(i,j, _currentLevelUsableTokens[dotToUse]) && maxIterations < 100)
                        {
                            maxIterations++;
                            dotToUse = Random.Range(0, _currentLevelUsableTokens.Length);
                        }
                        maxIterations = 0;

                        GameObject piece = Instantiate(_currentLevelUsableTokens[dotToUse], tempPosition, Quaternion.identity); // пул из массива с цветными токенами // можно добавит ьещё один массив с бонусами
                        piece.transform.parent = transform;
                        piece.name = "( " + i + ", " + j + " )";
                        currentLevelAllTokensArray[i, j] = piece;
                        piece.GetComponent<Dot>().row = j; // новые токены ползут сверху вниз
                        piece.GetComponent<Dot>().column = i; // новые токены ползут сверху вниз
                    }
                }
            }
        }

        //постоянный скан доски на наличие совпадений
        private bool MatchesOnBoard()
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if (currentLevelAllTokensArray[i,j] != null)
                    {
                        if (currentLevelAllTokensArray[i,j].GetComponent<Dot>().isMatched)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

    
        // =============================================================================================================
        // методы для проверки отсутствия совпадений на всей доске а так же поиск всех возможных совпадений на доске !!!


        private bool CheckForMatches()
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if (currentLevelAllTokensArray[i,j] != null)
                    {
                        // убедиться что проверяемые точки находятся в пределах игрового поля либо не зарезервированны
                        if (i < Width - 2)
                        {
                            // проверить существует ли точка справа и на две правее
                            if (currentLevelAllTokensArray[i + 1, j] != null && currentLevelAllTokensArray[i + 2, j] != null)
                            {
                                // потом отказаться от тегов // проверка наличие возможных совпадений вправо при возможном смещении точки
                                if (currentLevelAllTokensArray[i + 1, j].CompareTag(currentLevelAllTokensArray[i, j].tag) &&
                                    currentLevelAllTokensArray[i + 2, j].CompareTag(currentLevelAllTokensArray[i, j].tag))
                                {
                                    return true;
                                }
                            }
                        }
                        if (j < Height - 2)
                        {
                            // проверить существует ли точка выше и на две выше
                            if (currentLevelAllTokensArray[i, j + 1] != null && currentLevelAllTokensArray[i, j + 2] != null)
                            {
                                // потом отказаться от тегов // проверка наличие возможных совпадений вверх при возможном смещении точки
                                if (currentLevelAllTokensArray[i, j + 1].CompareTag(currentLevelAllTokensArray[i, j].tag) &&
                                    currentLevelAllTokensArray[i, j + 2].CompareTag(currentLevelAllTokensArray[i, j].tag))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        // во время работы метода FindAllMatces() из скрипта HintManager игра проверят каждую точку все доступные совпадения в разыне стороны
        // и запсиывает их в List в скрипте HintManager, там же выбирается случайная точка их этого списка и в данном
        // месте спавнится подсказка, в HintManager в апдейте идёт таймер бездейсивтя игрока
        public bool SwitchAndCheck(int column, int row, Vector2 direction) 
        {
            SwitchPieces(column, row, direction);
            if (CheckForMatches())
            {
                SwitchPieces(column, row, direction);
                return true;
            }
            SwitchPieces(column, row, direction);
            return false;
        }

        bool IsDeadlocked() // изменено Vid 52
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if (currentLevelAllTokensArray[i,j] != null)
                    {
                        if (i < Width - 1)
                        {
                            if (SwitchAndCheck(i, j, Vector2.right))
                            {
                                return false;
                            }                        
                        }
                        if (j < Height - 1)
                        {
                            if (SwitchAndCheck(i, j, Vector2.up))
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        // изменено Vid 51.1 (8 min)
        private void SwitchPieces(int column, int row, Vector2 direction)
        {
            if (currentLevelAllTokensArray[column + (int)direction.x, row + (int)direction.y] != null)
            {
                // взять второй токен и сохранить его в holder
                GameObject holder = currentLevelAllTokensArray[column + (int)direction.x, row + (int)direction.y] as GameObject; // лучше использовать as при извлечении одного объекта из двумерного массива
                // переключение позиции первой точки на позицию второй точки
                currentLevelAllTokensArray[column + (int)direction.x, row + (int)direction.y] = currentLevelAllTokensArray[column, row];
                // установить первую точку на позицию второй точки
                currentLevelAllTokensArray[column, row] = holder;
            }
        }

        private void ShuffleBoard()
        {
            List<GameObject> newBoard = new List<GameObject>();
            // добавить каждый активный токен в новый Лист
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if (currentLevelAllTokensArray[i, j] != null) // не берём зарезервированные места на доске
                    {
                        newBoard.Add(currentLevelAllTokensArray[i, j]);
                    }
                }
            }

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    //если место не зарезервировано
                    if (!_blankSpaces[i, j] && !_concreteTiles[i, j] && !_concreteTiles[i, j])
                    {
                        // Выбрать слуайное число
                        int pieceToUse = Random.Range(0, newBoard.Count);
                        //вызов проверки на совпадение при создании доски (не должно быть готовых совпадений)
                        int maxIterations = 0;

                        //============================================================
                        // механика которая при окончании времени не прекращает игру а перестаёт перезаполнять досту даёт сбой здесь
                        // при попытке перемешать массв с точками программа обращается к пустым элементам массива в которых предпложитено должны
                        // быть точки, игра не знает о том что массив точек перестал фактически заполнятся если закоментировать строки в Методе RefillBoarb
                        while (CheckForSameTokensAround(i, j, newBoard[pieceToUse]) && maxIterations < 100) // скокль попыток произведёт юнити для перетасовки доски что бы в ней заранее не оказалось совпадений
                            // 100 это магическое число служащее для того что бы не попасть в бесконечный цикл и в крайнем слуае создать новую доску с совпадениями 
                        {
                            pieceToUse = Random.Range(0, newBoard.Count);
                            maxIterations++;
                        }
                        // контенер для токена
                        Dot piece = newBoard[pieceToUse].GetComponent<Dot>();
                        maxIterations = 0;

                        // назначить позиции токену над которым ведется работа на данной итерации
                        piece.column = i;
                        piece.row = j;
                        // добавить/заменить рабочий токен в основной массив со всеми элементами доски
                        currentLevelAllTokensArray[i, j] = newBoard[pieceToUse];
                        // удалить текущий токен по адресу спика, во избежании повторного общащения к нему
                        newBoard.Remove(newBoard[pieceToUse]);
                    }
                }
            }
            //прежде чем выйдти из данного метода и завершить перетасовку необходимо проверить не будет ли перетасованная доска снова не иметь ходов
            if (IsDeadlocked())
            {
                // рекурсивный вызов
                ShuffleBoard();
            }

        }
    }
}