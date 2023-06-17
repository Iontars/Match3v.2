using System.Collections;
using UnityEngine;

namespace Base_Game_Scripts
{
    /// <summary>
    /// 
    /// </summary>

    public class Dot : MonoBehaviour
    {    
        [Header("Board Variables")]
        public int column;
        public int row;
        public int previousColumn, previousRow;
        public int targetX, targetY;[Space]
        [HideInInspector]public bool isMatched = false;

        private int tokenSpeed = 10;

        Animator animator;
        EndGameManager endGameManager;
        FindMatches findMatches;
        Board board;
        HintManager hintManager;
        [HideInInspector]public GameObject otherDot;
        Vector2 firstTouchPosition;
        Vector2 finalTouchPosition;
        Vector2 tempPosition;

        [Header("Swipe Stuff")]
        public float swipeAngle = 0;
        public float swipeResist = 1f; // сопротивление свайпу

        [Header("PowerUp Stuff")]
        public bool isColorBomb;
        public bool isColumnBomb;
        public bool isRowBomb;
        public bool isAjacentBomb;
        public GameObject ajacentMarker;
        public GameObject rowArrow;
        public GameObject columnArrow;
        public GameObject colorBomb;

        [Header("Animator Stuff")]
        Animator anim;
        float shineDelay;
        float shineDelaySeconds;


        void Awake()
        {
            endGameManager = FindObjectOfType<EndGameManager>();
            findMatches = FindObjectOfType<FindMatches>();
            anim = GetComponent<Animator>();
            //hintManager = FindObjectOfType<HintManager>();
            // СДЕЛАТЬ ТАКУЮ СИСТЕМУ ПОИСКЕ ПОМПОНЕНТОВ ВЕЗДЕ
            hintManager = GameObject.FindGameObjectWithTag("Board").GetComponent<HintManager>(); ;
            GameObject.FindGameObjectWithTag("Board").TryGetComponent(out board);
            //board = FindObjectOfType<Board>();       
        }

        void Start()
        {
            isColorBomb = false;
            isColumnBomb = false;
            isRowBomb = false;
            isAjacentBomb = false;
            shineDelay= Random.Range(2,5); // так как точки создаются часто то рандомное значение можно задать единожды в конструкторе Start
            shineDelaySeconds = shineDelay;
        }
        
        void Update()
        {
            // создание рандомных интервалов запуска анимации, можно использовать для других целей
            shineDelaySeconds -= Time.deltaTime;
            if (shineDelaySeconds <= 0)
            {
                shineDelaySeconds = shineDelay;
                StartCoroutine(nameof(StartShine));
            }

            targetX = column;
            targetY = row;
            
            if (Mathf.Abs(targetX - transform.position.x) > .1f)
            {
                tempPosition = new Vector2(targetX, transform.position.y);
                transform.position = Vector2.Lerp(transform.position, tempPosition, tokenSpeed * Time.deltaTime);
                if (board.currentLevelAllTokensArray[column, row] != gameObject)// падение токенов после уничтожения совпавших
                {
                    board.currentLevelAllTokensArray[column, row] = gameObject;
                }
                findMatches.FindAllMatches();
            }
            else
            {
                tempPosition = new Vector2(targetX, transform.position.y);
                transform.position = tempPosition;           
            }

            if (Mathf.Abs(targetY - transform.position.y) >.1f)
            {
                tempPosition = new Vector2(transform.position.x, targetY);
                transform.position = Vector2.Lerp(transform.position, tempPosition, tokenSpeed * Time.deltaTime);
                if (board.currentLevelAllTokensArray[column, row] != gameObject)// падение токенов после уничтожения совпавших
                {
                    board.currentLevelAllTokensArray[column, row] = gameObject;
                }
                findMatches.FindAllMatches();
            }
            else
            {
                tempPosition = new Vector2(transform.position.x, targetY);
                transform.position = tempPosition;
            }
        }
        
        IEnumerator StartShine()
        {
            anim.SetBool("Shine", true);
            yield return null; // подождать 1 кадр
            anim.SetBool("Shine", false);

        }

        public void PopAnimation()
        {
            anim.SetBool("Popped", true);
        }

        //проверка на возврат токенов на начальые места если нет совпадений
        public IEnumerator CheckMoveCo()
        {
            // отношение к бонусам цветная бомба
            if (isColorBomb)
            {
                findMatches.MatchPiecesOfColor(otherDot.tag);
                isMatched = true;
            }
            else if (otherDot.GetComponent<Dot>().isColorBomb)
            {
                findMatches.MatchPiecesOfColor(gameObject.tag);
                otherDot.GetComponent<Dot>().isMatched = true;
            }
            //

            yield return new WaitForSeconds(.5f); // задержка для того что бы отработал глупыей код в апдейте // перестроить зависимости

            if (otherDot != null)
            {
                //cсовпадение не произошло // создать ивент?
                if (!isMatched && !otherDot.GetComponent<Dot>().isMatched)
                {
                    otherDot.GetComponent<Dot>().row = row;
                    otherDot.GetComponent<Dot>().column = column;
                    row = previousRow;
                    column = previousColumn;
                    yield return new WaitForSeconds(.5f);
                    board.currentDot = null;
                    board.currentState = GameState.Move;
                }
                //cсовпадение произошло // создать ивент?
                else
                {
                    // если режим игры на карте помечен как игра на количество ходов то запустится метод отнимающий один ход
                    if (endGameManager?.requirements.gameType == GameType.Moves)
                    {
                        endGameManager.DecreaseCountervalue();
                    }
                    // уничтожаем все точки отмеенных как IsMatch 
                    board?.DestroyMatches();                
                }
            }

        }


        private void OnMouseDown()
        {
            // анимация прикосновения
            if (anim != null)
            {
                anim.SetBool("Touched", true);
            }

            // отношение к уничтожении подсказки
            if (hintManager != null)
            {
                hintManager.DestroyHint();
            }

            if (board.currentState == GameState.Move)
            {
                firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); 
            }
        }

        private void OnMouseUp()
        {
            // анимация прикосновения
            if (anim != null)
            {
                anim.SetBool("Touched", false);
            }

            if (board.currentState == GameState.Move)
            {
                finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); 
                CalculateAngle();
            }

        }

        // расчёт радианы угла направления
        void CalculateAngle()
        {
            // если магнитуда свайпа больше swipeResist тогда произойдёт свайп
            if (Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist ||
                Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist)
            {
                board.currentState = GameState.Wait;
                swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180/ Mathf.PI;
                MovePicies();
                board.currentDot = this;
            }
            else
            {
                board.currentState = GameState.Move;
            }
        }

        // для тестов и отладки (под удаление)
        private void OnMouseOver()
        {

            //под удаление
            if (Input.GetMouseButtonDown(1))
            {
                isAjacentBomb = true;
                GameObject marker = Instantiate(ajacentMarker, transform.position, Quaternion.identity);
                marker.transform.parent = this.transform;
            }
        }
        
        // метод косвенно выполняющий перемещение точек // меняет позиции тайлов и те автоматически просто хоят твернуться на новые позиции, тупо
        void MovePiecesActual(Vector2 direction)
        {
            otherDot = board.currentLevelAllTokensArray[column + (int)direction.x, row + (int)direction.y];
            previousRow = row;
            previousColumn = column;

            // тайлам не присвоятся новые значеняи позиций если в позиции стартового тайла висит лок тайл, либо в конечной позиции тайла тоже висит лок тайл
            if (board.lockTiles[column, row] == null && board.lockTiles[column + (int)direction.x, row + (int)direction.y] == null)
            {
                if (otherDot != null) // проверка на зарезервировнные пустые тайлы внутри доски // 
                {
                    // можно двигаться так как соседний тайл существует
                    otherDot.GetComponent<Dot>().column += -1 * (int)direction.x;
                    otherDot.GetComponent<Dot>().row += -1 * (int)direction.y;
                    column += (int)direction.x;
                    row += (int)direction.y;
                    StartCoroutine(nameof(CheckMoveCo));
                }
                else
                {
                    board.currentState = GameState.Move; // сброс машины состояний после неудавшегося свайпа
                }
            }
            else
            {
                board.currentState = GameState.Move; // сброс машины состояний после неудавшегося свайпа
            }
        }

        // расчёт направления свайпа // лучше записать полученное направление свайпа в переменную, в дальнейшем это поможет избежать дублирования кода и ошибок. енам
        void MovePicies()
        {
            if (swipeAngle > -45 && swipeAngle <= 45 && column < board.Width -1 )
            {
                //свайп вправо
                MovePiecesActual(Vector2.right);
            }
            else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.Height - 1)
            {
                //свайп вверх
                MovePiecesActual(Vector2.up);
            }
            else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
            {
                //Левый свайп
                MovePiecesActual(Vector2.left);
            }
            else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
            {
                //Свайп вниз
                MovePiecesActual(Vector2.down); // если здесь поменять вектор на другой
                // например Vector.up  то это может запутать игрока, тем самым можно сделать
                // это как часть игровой механики в виде дебафа игрока
            }
            else
            {
                board.currentState = GameState.Move;

            }
        }

        public void MakeRowBomb()
        {
            if (!isColumnBomb && !isColorBomb && !isAjacentBomb)
            {
                isRowBomb = true;
                GameObject arrow = Instantiate(rowArrow, transform.position, Quaternion.identity);
                arrow.transform.parent = this.transform;
            }
        
        }

        public void MakeColumnBomb()
        {
            if (!isRowBomb && !isColorBomb && !isAjacentBomb)
            {
                isColumnBomb = true;
                GameObject arrow = Instantiate(columnArrow, transform.position, Quaternion.identity);
                arrow.transform.parent = this.transform;
            }
        }

        public void MakeColorBomb()
        {
            if (!isColumnBomb && !isRowBomb && !isAjacentBomb)
            {
                isColorBomb = true;
                GameObject color = Instantiate(colorBomb, transform.position, Quaternion.identity);
                color.transform.parent = this.transform;
                gameObject.tag = "Color";
            }
        }
        public void MakeAjacentBomb()
        {
            if (!isColumnBomb && !isColorBomb && !isRowBomb)
            {
                isAjacentBomb = true;
                GameObject marker = Instantiate(ajacentMarker, transform.position, Quaternion.identity);
                marker.transform.parent = this.transform;
            }
        }
    }
}
