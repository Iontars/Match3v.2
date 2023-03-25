﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Base_Game_Scripts
{
    /// <summary>
    /// 
    /// </summary>

    public class FindMatches : MonoBehaviour
    {
        Board board;
        // список хранящий колличество совпадений за 1 ход
        public List<GameObject> currentMatches = new List<GameObject>();  

        void Awake()
        {
            board = FindObjectOfType<Board>();
        }

        public void FindAllMatches() => StartCoroutine(nameof(FindAllMatchesCo));


        private List<GameObject> isAdjacentBomb(Dot dot1, Dot dot2, Dot dot3)
        {
            List<GameObject> currentDots = new List<GameObject>();
            if (dot1.isAjacentBomb)
            {
                currentMatches.Union(GetAdjacentPieces(dot1.column, dot1.row));
            }
            if (dot2.isAjacentBomb)
            {
                currentMatches.Union(GetAdjacentPieces(dot2.column, dot2.row));
            }
            if (dot3.isAjacentBomb)
            {
                currentMatches.Union(GetAdjacentPieces(dot3.column, dot3.row));
            }
            return currentDots;
        }
        // изменено объеснено Vid 52.1 19 min
        private List<GameObject> isRowBomb(Dot dot1, Dot dot2, Dot dot3)
        {
            List<GameObject> currentDots = new List<GameObject>();
            if (dot1.isRowBomb)
            {
                currentMatches.Union(GetRowPieces(dot1.row));
                board.BombRow(dot1.row); // связано с уничтожением бетона, обращаемся к точке в массиве горизонтальных бомб и прозваниваем в 4 направления на наличие бетона рядом
            }
            if (dot2.isRowBomb)
            {
                currentMatches.Union(GetRowPieces(dot2.row));
                board.BombRow(dot2.row);
            }
            if (dot3.isRowBomb)
            {
                currentMatches.Union(GetRowPieces(dot3.row));
                board.BombRow(dot3.row);
            }
            return currentDots;
        }

        private List<GameObject> isColumnBomb(Dot dot1, Dot dot2, Dot dot3)
        {
            List<GameObject> currentDots = new List<GameObject>();
            if (dot1.isColumnBomb)
            {
                currentMatches.Union(GetColumnPieces(dot1.column));
                board.BombColumn(dot1.column);
            }
            if (dot2.isColumnBomb)
            {
                currentMatches.Union(GetColumnPieces(dot2.column));
                board.BombColumn(dot1.column);
            }
            if (dot3.isColumnBomb)
            {
                currentMatches.Union(GetColumnPieces(dot3.column));
                board.BombColumn(dot1.column);
            }
            return currentDots;
        }

        private void AddToListAndMatch(GameObject dot)
        {
            if (!currentMatches.Contains(dot))
            {
                currentMatches.Add(dot);
            }
            dot.GetComponent<Dot>().isMatched = true; // отмечает точку попавшую в список как isMatched, что позволяет уничтожать ряд совпадений
        }

        private void GetNearbyPieces(GameObject dot1, GameObject dot2, GameObject dot3)
        {
            AddToListAndMatch(dot1);
            AddToListAndMatch(dot2);
            AddToListAndMatch(dot3); 
        }


        // проверка на совпадения токенов
        IEnumerator FindAllMatchesCo()
        {
            //yield return new WaitForSeconds(.2f);
            yield return null;   
            for (int i = 0; i < board.Width; i++)
            {
                for (int j = 0; j < board.Height; j++)
                {
                    GameObject currentDot = board.currentLevelAllTokensArray[i, j];
                
                    if (currentDot != null)
                    {
                        Dot currentDotDot = currentDot.GetComponent<Dot>();
                        if (i > 0 && i < board.Width - 1)
                        {
                            GameObject leftDot = board.currentLevelAllTokensArray[i - 1, j];
                            GameObject rightDot = board.currentLevelAllTokensArray[i + 1, j];
                            if (leftDot != null && rightDot != null)
                            {
                                Dot leftDotDot = leftDot.GetComponent<Dot>();
                                Dot rightDotDot = rightDot.GetComponent<Dot>();
                                if (leftDot != null && rightDot != null)
                                {
                                    if (leftDot.tag == currentDot.tag &&
                                        rightDot.tag == currentDot.tag)
                                    {
                                        currentMatches.Union(isRowBomb(leftDotDot, rightDotDot, currentDotDot));
                                        currentMatches.Union(isColumnBomb(leftDotDot, rightDotDot, currentDotDot));
                                        currentMatches.Union(isAdjacentBomb(leftDotDot, rightDotDot, currentDotDot));
                                     
                                        GetNearbyPieces(leftDot, rightDot, currentDot); // если все соседние точки одинаковые то передаём их в метод добавлящий их в список в котором они будут отмечены как isMatched
                                    }
                                }
                            }
                        }
                        if (j > 0 && j < board.Height - 1)
                        {
                            GameObject downDot = board.currentLevelAllTokensArray[i, j - 1];
                            GameObject upDot = board.currentLevelAllTokensArray[i, j + 1];

                            if (downDot != null && upDot != null)
                            {
                                Dot downDotDot = downDot.GetComponent<Dot>();
                                Dot upDotDot = upDot.GetComponent<Dot>();
                                if (downDot != null && upDot != null)
                                {
                                    if (downDot.tag == currentDot.tag &&
                                        upDot.tag == currentDot.tag)
                                    {
                                        currentMatches.Union(isRowBomb(upDotDot, downDotDot, currentDotDot));
                                        currentMatches.Union(isColumnBomb(upDotDot, downDotDot, currentDotDot));
                                        currentMatches.Union(isAdjacentBomb(upDotDot, downDotDot, currentDotDot));
                                        GetNearbyPieces(upDot, downDot, currentDot);

                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        // совпадения для цветной бомбы
        public void MatchPiecesOfColor( string color )
        {
            for (int i = 0; i < board.Width; i++)
            {
                for (int j = 0; j < board.Height; j++)
                {
                    // Проверить вызывается ли этот кусок
                    if (board.currentLevelAllTokensArray[i,j] != null)
                    {
                        if (board.currentLevelAllTokensArray[i,j].tag == color)
                        {
                            board.currentLevelAllTokensArray[i, j].GetComponent<Dot>().isMatched = true;
                        }
                    }

                }
            }
        }
        // Совпадение для Большой бомбы
        List<GameObject> GetAdjacentPieces(int column, int row)
        {
            List<GameObject> dots = new List<GameObject>();
            for (int i = column - 1 ; i <= column + 1; i++)
            {
                for (int j = row - 1; j < row + 1; j++)
                {
                    // находится ли фигура внутри доски
                    if (i >= 0 && i < board.Width && j >= 0 && j < board.Height)
                    {
                        if (board.currentLevelAllTokensArray[i,j] != null) // проверка на наличие точек вокруг радиуса взрыва, что бы не вызвать ошибку исключения
                        {
                            dots.Add(board.currentLevelAllTokensArray[i, j]);
                            board.currentLevelAllTokensArray[i, j].GetComponent<Dot>().isMatched = true;
                        }
                    }

                }
            }
            return dots;
        }

        //запись всех вертикальных токенов в список для возможности их уничтожения бонусом
        List<GameObject> GetColumnPieces(int column)
        {
            List<GameObject> dots = new List<GameObject>();
            for (int i = 0; i < board.Height; i++)
            {
                if (board.currentLevelAllTokensArray[column,i] != null)
                {
                    Dot dot = board.currentLevelAllTokensArray[column, i].GetComponent<Dot>();
                    if (dot.isRowBomb) // если среди задетых токенов, взрывной волной есть горизонтальная бомба то
                    {
                        dots.Union(GetRowPieces(i)).ToList(); // если взрывная волна от вертикальной бомбы задевает горизонральную бомбу она тоже взрывается
                    }
                    dots.Add(board.currentLevelAllTokensArray[column, i]);
                    dot.isMatched = true;
                }
            }
            return dots;
        }
        //запись всех горизонтальных токенов в список для возможности их уничтожения бонусом
        List<GameObject> GetRowPieces(int row)
        {
            List<GameObject> dots = new List<GameObject>();
            for (int i = 0; i < board.Width; i++)
            {
                if (board.currentLevelAllTokensArray[i, row] != null)
                {
                    Dot dot = board.currentLevelAllTokensArray[i, row].GetComponent<Dot>();
                    if (dot.isColumnBomb) // если среди задетых токенов, взрывной волной есть вертикальная бомба то
                    {
                        dots.Union(GetColumnPieces(i)).ToList(); // если взрывная волна от горизонтальной бомбы задевает вертикальную бомбу она тоже взрывается
                    }
                    dots.Add(board.currentLevelAllTokensArray[i, row]);
                    dot.isMatched = true;
                }
            }
            return dots;
        }

        public void CheckBombs(MatchType matchType)
        {
            // игрок что то передвигает ?
            if (board.currentDot != null)
            {
                // передвигаимая фигура совпадает ?
                if (board.currentDot.isMatched && board.currentDot.tag == matchType.color)
                {
                    // сделать токен не разрушаимым
                    board.currentDot.isMatched = false;

                    //в зависимости от сделанного направления свайпа спавним тип строковой бомбы // лучше изначально напсиать направление свайпа в переменную что бы не дублировать код
                    if ((board.currentDot.swipeAngle > - 45 && board.currentDot.swipeAngle <= 45)||
                        (board.currentDot.swipeAngle < -135 || board.currentDot.swipeAngle >= 135))
                    {
                        board.currentDot.MakeColumnBomb();
                    }
                    else
                    {
                        board.currentDot.MakeRowBomb();
                    }
                }
                // другой токен совпадает ?
                else if (board.currentDot.otherDot != null)
                {
                    Dot otherDot = board.currentDot.otherDot.GetComponent<Dot>();
                    if (otherDot.isMatched && otherDot.tag == matchType.color)
                    {
                        otherDot.isMatched = false;

                        if ((board.currentDot.swipeAngle > -45 && board.currentDot.swipeAngle <= 45) ||
                            (board.currentDot.swipeAngle < -135 || board.currentDot.swipeAngle >= 135))
                        {
                            otherDot.MakeRowBomb();
                        }
                        else
                        {
                            otherDot.MakeColumnBomb();
                        }
                    }
                }
            }
        }
    }
}
