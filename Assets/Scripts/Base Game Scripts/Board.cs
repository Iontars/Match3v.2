using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState{wait, move, win, lose, pause} // состояние используется для блокировки повторного свайпа пока токены движутся и меняет состояние игры

public enum TileKind {Breakable, Blank, Normal } // состояние тайлов, говорящще о том занял ли тайл или нет

[System.Serializable]
public class TileType // класс хранящйи в себе информацию о том занят ли тайл или нет
{
    public int x;
    public int y;
    public TileKind tileKind;
}

/// <summary>
/// Постороение игрового поля, создание токенов, поиск различных совпадений, уничтожение токенов, звук токенов,
/// добавление очков за токены, распознавание пустых мест а так же границы доски, перезаполнение доски.
/// </summary>
public class Board : MonoBehaviour
{
    [Header("SO Sfuff")]
    public World world;
    public int level;
    public GameState currentState = GameState.move; // ?Рудимент

    [Header("Board Dimension")]
    public int width;
    public int height;
    public int offSet; // смещает позициаю спавна точек по оси х что позволяет им скользить вниз при появлении

    [Header("Prefabs")]
    public GameObject tilePrefab;
    public GameObject breakebleTilePrefab;
    public GameObject[] dots;
    public GameObject destroyEffect;

    [Header("Layout")]
    public TileType[] boardLayout;
    private bool[,] blankSpaces; // массив с зарезервированными местами на доске
    private BackgroundTile[,] breakableTiles; // массив с ломающимися плитками на доске
    public GameObject[,] allDots; // массив со всеми игровыми точками на доске
    public Dot currentDot; // запись в скрипте Dot метод CalculateAngle
    FindMatches findMatches;
    public int basePieceValue = 5;
    int streakValue = 1;
    ScoreManager scoreManager;
    SoundManager soundManager;

    GoalManager goalManager; // vid 40
    public int[] scoreGoals; // сколько нужно набрать очков для различных успехов на карте, 1 звезда 2000 очков 2 звезды 4000 очков итд

    public float refillDelay = 0.7f;

    private void Awake()
    {
        //загрузка номера уровня переданного из сцены выбора уровней
        if (PlayerPrefs.HasKey(PlayerPrefsStorage.keyCurrentLevel))
        {
            level = PlayerPrefs.GetInt(PlayerPrefsStorage.keyCurrentLevel);
        }

        if (world != null)
        {
            if (level < world.levels.Length)
            {
                if (world.levels[level] != null)
                {
                    //присваивание значений из выбранного уровня в нашу доску
                    width = world.levels[level].width;
                    height = world.levels[level].height;
                    dots = world.levels[level].dots;
                    scoreGoals = world.levels[level].scoreGoals;
                    boardLayout = world.levels[level].boardLayout;
                } 
            }
        }
        
        scoreManager = FindObjectOfType<ScoreManager>();
        goalManager = FindObjectOfType<GoalManager>();
        soundManager = FindObjectOfType<SoundManager>();
        blankSpaces = new bool[width, height];
        findMatches = FindObjectOfType<FindMatches>();
        breakableTiles = new BackgroundTile[width, height];       
        allDots = new GameObject[width, height];
    }

    void Start()
    {
        currentState = GameState.pause; // игра начинается с состояния паузы !!!!!!!!!!!!
        SetUp();
    }

    // Создание пустых/занятых мест на доске, для разнообразия геймплея
    public void GenerateBlankSpaces()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            // мы не проверям позицию зарезервированного места, мы проверяем Emum позицию в массиве перечислений
            if (boardLayout[i].tileKind == TileKind.Blank ) // если место пустое
            {
                blankSpaces[boardLayout[i].x, boardLayout[i].y] = true; // запистаь в булевй массив позицию пустого места и сделать true
            }
        }
    }

    // метод создания ломающихся плиток
    public void GenerateBreakableTiles()
    {
        // просмотреть все плитки назначенные на ломающиеся в классе TileType
        for (int i = 0; i < boardLayout.Length; i++)
        {
            // если по данному адресу прописана ломающаяся плитка
            if (boardLayout[i].tileKind == TileKind.Breakable)
            {
                // заспавнить ломающуюся плитку
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y); // вот здесь мы получаем координаты указанные в инспекоре по позициям x и y
                GameObject tile = Instantiate(breakebleTilePrefab, tempPosition, Quaternion.identity);
                breakableTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();

            }
        }
    }

    //создание доски
    void SetUp()
    {
        GenerateBlankSpaces(); // перед созданием тайлов на доске, создаём зарезервированные места
        GenerateBreakableTiles(); // генерация ломающихся плиток
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {

                if (!blankSpaces[i, j]) // проверка на пустое место на доске/если false создаёт тайл на доске
                {
                    Vector2 tempPosition = new Vector2(i, j);
                    Vector2 tilePosition = new Vector2(i, j);
                    GameObject backgroundTile = Instantiate(tilePrefab, tilePosition, Quaternion.identity) as GameObject;
                    backgroundTile.transform.parent = transform;
                    backgroundTile.name = "( " + i + ", " + j + " )";
                    int dotToUse = Random.Range(0, dots.Length);

                    //вызов проверки на совпадение при создании доски (не должно быть готовых совпадений)
                    int maxIterations = 0;
                    while (MatchesAt(i, j, dots[dotToUse]) && maxIterations < 100)
                    {
                        dotToUse = Random.Range(0, dots.Length);
                        maxIterations++;
                        //Debug.Log(maxIterations);
                    }
                    maxIterations = 0;

                    GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    dot.GetComponent<Dot>().row = j;
                    dot.GetComponent<Dot>().column = i;
                    dot.transform.parent = transform;
                    dot.name = "( " + i + ", " + j + " )";
                    allDots[i, j] = dot;
                }
            }
        }
    }

    //проверка на одинаковые токены при создании доски
    bool MatchesAt(int colunm, int row, GameObject piece)
    {
        if (colunm > 1 && row >1 )
        {
            if (allDots[colunm -1, row] != null && allDots[colunm - 2, row] != null) //проверка относящаяся к зарезервированым местам на доске
            {
                if (allDots[colunm - 1, row].tag == piece.tag &&
                        allDots[colunm - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }
            if (allDots[colunm, row - 1] != null && allDots[colunm, row - 2] != null) //проверка относящаяся к зарезервированым местам на доске
            {
                if (allDots[colunm, row - 1].tag == piece.tag &&
                        allDots[colunm, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }
        }
        else if(colunm <= 1 || row <= 1)
        {
            if (row > 1)
            {
                if (allDots[colunm, row - 1] != null && allDots[colunm, row - 2] != null) //проверка относящаяся к зарезервированым местам на доске
                {
                    if (allDots[colunm, row - 1].tag == piece.tag &&
                                allDots[colunm, row - 2].tag == piece.tag)
                    {
                        return true;
                    }
                }
            }
            if (colunm > 1) //
            {
                if (allDots[colunm - 1, row] != null && allDots[colunm - 2, row] != null) //проверка относящаяся к зарезервированым местам на доске
                {
                    if (allDots[colunm - 1, row].tag == piece.tag &&
                                allDots[colunm - 2, row].tag == piece.tag)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private int ColumnOrRow()
    {
        // создать копию FindMatches.currentMatches
        List<GameObject> matchCopy = findMatches.currentMatches as List<GameObject>; // as Важно изучить

        // Просмотреть список и решить нужно ли делать бомбу
        for (int i = 0; i < matchCopy.Count; i++)
        {
            // сохраним текущую точку
            Dot thisDot = matchCopy[i].GetComponent<Dot>();
            int column = thisDot.column;
            int row = thisDot.row;
            int columnMatch = default;
            int rowMatch = default;
            // просмотреть остальные соседние точки и сравнить
            for (int j = 0; j < matchCopy.Count; j++)
            {
                Dot nextDot = matchCopy[j].GetComponent<Dot>();
                if (thisDot == nextDot)
                {
                    continue;
                }
                if (thisDot.column == nextDot.column && nextDot.CompareTag(thisDot.tag)) // перейти на компонентную систему заменив теги
                {
                    columnMatch++;
                }
                if (thisDot.row == nextDot.row && nextDot.CompareTag(thisDot.tag)) // перейти на компонентную систему заменив теги
                {
                    rowMatch++;
                }
            }
            //return 3 если колонки или строки совпали
            //return 2 если бомба уничтожающая соседние тайлы
            //return 1 если цветная бомба
            if (columnMatch == 4 || rowMatch == 4) //  сравниваем с числом меньше фактического так как не учитываем сами себя
            {
                return 1;
            }
            if (columnMatch == 2 && rowMatch == 2)
            {
                return 2;
            }
            if (columnMatch == 3 || rowMatch == 3)
            {
                return 3;
            }          
        }
        return 0;
    }


    // проверить какой тип бонуса создать при совпадениях
    private void CheckToMakeBombs()
    {
        // Скольк осовпадений за раз находится в списке FindMatches.currentMatches
        if (findMatches.currentMatches.Count > 3)
        {
            // какой тип срвпадений?
            int typeOfMatch = ColumnOrRow();
            if (typeOfMatch == 1)
            {
                // Создать цветную бомбу
                // Текущая точка совпала ?
                if (currentDot != null)
                {
                    if (currentDot.isMatched)
                    {
                        if (!currentDot.isColorBomb)
                        {
                            currentDot.isMatched = false;
                            currentDot.MakeColorBomb();
                        }
                    }
                    else
                    {
                        if (currentDot.otherDot != null)
                        {
                            Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                            if (otherDot.isMatched)
                            {
                                if (!otherDot.isColorBomb)
                                {
                                    otherDot.isMatched = false;
                                    otherDot.MakeColorBomb();
                                }
                            }
                        }
                    }
                }
                
            }
            else if (typeOfMatch == 2)
            {
                // Создать Звезду
                if (currentDot != null)
                {
                    if (currentDot.isMatched)
                    {
                        if (!currentDot.isAjacentBomb)
                        {
                            currentDot.isMatched = false;
                            currentDot.MakeAjacentBomb();
                        }
                    }
                    else
                    {
                        if (currentDot.otherDot != null)
                        {
                            Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                            if (otherDot.isMatched)
                            {
                                if (!otherDot.isAjacentBomb)
                                {
                                    otherDot.isMatched = false;
                                    otherDot.MakeAjacentBomb();
                                }
                            }
                        }
                    }
                }
            }
            else if (typeOfMatch == 3)
            {
                findMatches.CheckBombs();
            }

        }

    }

    // Уничтожение совпавших токенов // тут же подсчёт очков // звук ломания токена выделить в отдельный ивент/метод/ партикл взрыва
    void DestroyMatchesAt(int colunm, int row)
    {
        if (allDots[colunm,row].GetComponent<Dot>().isMatched)
        {
            // сколько эллементов в Списке currentMatches ?
            if (findMatches.currentMatches.Count >=4)
            {
                CheckToMakeBombs();
            }
            // нужно ли разбивать ломающуюся плитку
            if (breakableTiles[colunm, row] != null)
            {
                // если это нанесёт 1 единицу урона
                breakableTiles[colunm, row].TakeDamage(1);
                if (breakableTiles[colunm, row].hitPoints <= 0)
                {
                    breakableTiles[colunm, row] = null; // удаляем из массива ломающийся токен
                }
            }

            // vid 40 // сравниваем ломающуюся плитку на предмет цели уровня
            if (goalManager != null)
            {
                goalManager.CompareGoal(allDots[colunm, row].tag.ToString());
                goalManager.UpdateGoals();
            }

            soundManager?.PlayRandomDestroyNoise(); // звук ломания токена

            GameObject particle = Instantiate(destroyEffect, new Vector3( // анимация взрыва партиклом
                allDots[colunm, row].transform.position.x,
                allDots[colunm, row].transform.position.y,
                allDots[colunm, row].transform.position.z - 1), Quaternion.identity);
            Destroy(particle, .3f);
            allDots[colunm,row].GetComponent<Dot>().PopAnimation(); // анимация спрайта
            Destroy(allDots[colunm, row], .3f); // Фактическое уничтожение совпавших бомб
            scoreManager.IncreaseScore(basePieceValue * streakValue); // добавление очков на табло
            allDots[colunm, row] = null;
            
        }
    }

    // вызов Уничтожение совпавших токенов
    public void DestroyMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i,j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }
        findMatches.currentMatches.Clear();
        StartCoroutine(nameof(DecreaseRowCo2));
    }

    // продвинутое распознавание пустых мест на доске
    IEnumerator DecreaseRowCo2()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                // не зарезервировано ли место внутри доски?
                if (!blankSpaces[i,j] && allDots[i,j] == null)
                {
                    // проверка доски
                    for (int k = j + 1; k < height; k++)
                    {
                        // найдена ли точка ?
                        if (allDots[i,k] != null)
                        {
                            // переместить точки в пустое пространство
                            allDots[i, k].GetComponent<Dot>().row = j;
                            // установить значение этого места как null
                            allDots[i, k] = null;
                            break;
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(refillDelay * 0.5f);
        StartCoroutine(nameof(FillBoardCo));
    }

    //распознавание пустых мест на доске
    IEnumerator DecreaseRowCo()
    {
        int nullCount = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null)
                {
                    nullCount++;
                }
                else if (nullCount > 0)
                {
                    allDots[i, j].GetComponent<Dot>().row -= nullCount;
                    allDots[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(refillDelay * 0.5f);
        StartCoroutine(nameof(FillBoardCo));
    }

    // заполнение пустых ячеек
    void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i,j] == null && !blankSpaces[i,j]) // проверка в том числе на зарезервированные места на доске
                {
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    int dotToUse = Random.Range(0, dots.Length); // пул из массива с цветными токенами // можно добавит ьещё один массив с бонусами

                    //фикс проблемы когда при каскаде можно было передвиграть фгуры вручную
                    int maxIterations = 0;
                    while (MatchesAt(i,j, dots[dotToUse]) && maxIterations < 100)
                    {
                        maxIterations++;
                        dotToUse = Random.Range(0, dots.Length);
                    }
                    maxIterations = 0;

                    GameObject piece = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    //piece.transform.parent = transform;
                    //piece.name = "( " + i + ", " + j + " )";
                    allDots[i, j] = piece;
                    piece.GetComponent<Dot>().row = j; // новые токены ползут сверху вниз
                    piece.GetComponent<Dot>().column = i; // новые токены ползут сверху вниз
                }
            }
        }
    }

    //постоянный скан доски на наличие совпадений
    bool MatchesOnBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i,j] != null)
                {
                    if (allDots[i,j].GetComponent<Dot>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    IEnumerator FillBoardCo()
    {
        RefillBoard(); // заполнить досу токенами после первого совпадения
        yield return new WaitForSeconds(refillDelay); // и подождать
        // каскад совпадений
        while (MatchesOnBoard()) // пока есть каскад совпадений, ждать wait
        {
            streakValue ++; // серия совпадений
            //currentState = GameState.wait;
            print(streakValue);
            DestroyMatches(); // вызов этого метода должен быть раньше задержки
            yield return new WaitForSeconds(1.5f * refillDelay); // нужно дождаться заполнения доски прежде чем проверить поэтому увеличиваем время
        }
        findMatches.currentMatches.Clear(); // Имеет отношенеи к бонусам
        yield return new WaitForSeconds(refillDelay);

        if (IsDeadlocked()) // проверка на наличие того что на доске больше нельзя создать совпадений
        {
            ShuffleBorad();
        }
        if (!MatchesOnBoard())
        {
            currentState = GameState.move;
            streakValue = 1;
        }
    }

    // методы для проверки отсутствия совпадений на всей доске !!!

    void SwitchPieces(int column, int row, Vector2 direction)
    {
        // взять второй токен и сохранить его в holder
        GameObject holder = allDots[column + (int)direction.x, row + (int)direction.y] as GameObject; // лучше использовать as при извлечении одного объекта из двумерного массива
        // переключение позиции первой точки на позицию второй точки
        allDots[column + (int)direction.x, row + (int)direction.y] = allDots[column, row];
        // установить первую точку на позицию второй точки
        allDots[column, row] = holder;
    }

    bool CheckForMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i,j] != null)
                {
                    // убедиться что проверяемые точки находятся в пределах игрового поля либо не зарезервированны
                    if (i < width - 2)
                    {
                        // проверить существует ли точка справа и на две правее
                        if (allDots[i + 1, j] != null && allDots[i + 2, j] != null)
                        {
                            // потом отказаться от тегов // проверка наличие возможных совпадений вправо при возможном смещении точки
                            if (allDots[i + 1, j].tag == allDots[i, j].tag &&
                                allDots[i + 2, j].tag == allDots[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }
                    if (j < height - 2)
                    {
                        // проверить существует ли точка выше и на две выше
                        if (allDots[i, j + 1] != null && allDots[i, j + 2] != null)
                        {
                            // потом отказаться от тегов // проверка наличие возможных совпадений вверх при возможном смещении точки
                            if (allDots[i, j + 1].tag == allDots[i, j].tag &&
                                allDots[i, j + 2].tag == allDots[i, j].tag)
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

    bool IsDeadlocked()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i,j] != null)
                {
                    if (i < width - 1)
                    {
                        if (SwitchAndCheck(i, j, Vector2.right))
                        {
                            return false;
                        }                        
                    }
                    if (j < height - 1)
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

    void ShuffleBorad()
    {
        List<GameObject> newBoard = new List<GameObject>();
        // добавить каждый активный токен в новый Лист
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i,j] != null) // не берём зарезервированные места на доске
                {
                    newBoard.Add(allDots[i, j]);
                }
            }
        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                //если место не зарезервировано
                if (!blankSpaces[i,j])
                {
                    // Выбрать слуайное число
                    int pieceToUse = Random.Range(0, newBoard.Count);

                    //вызов проверки на совпадение при создании доски (не должно быть готовых совпадений)
                    int maxIterations = 0;
                    while (MatchesAt(i, j, newBoard[pieceToUse]) && maxIterations < 100)
                    {
                        pieceToUse = Random.Range(0, newBoard.Count);
                        maxIterations++;
                        //Debug.Log(maxIterations);
                    }
                    // контенер для токена
                    Dot piece = newBoard[pieceToUse].GetComponent<Dot>();
                    maxIterations = 0;

                    // назначить позиции токену над которым ведется работа на данной итерации
                    piece.column = i;
                    piece.row = j;
                    // добавить/заменить рабочий токен в основной массив со всеми элементами доски
                    allDots[i, j] = newBoard[pieceToUse];
                    // удалить текущий токен по адресу спика, во избежании повторного общащения к нему
                    newBoard.Remove(newBoard[pieceToUse]);
                }
            }
        }
        //прежде чем выйдти из данного метода и завершить перетасовку необходимо проверить не будет ли перетасованная доска снова не иметь ходов
        if (IsDeadlocked())
        {
            // рекурсивный вызов
            ShuffleBorad();
        }
    }

    private void Update()
    {
        
    }
}
