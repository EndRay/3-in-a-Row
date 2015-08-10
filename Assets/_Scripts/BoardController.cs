using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using DG.Tweening;
using UnityEngine;

public class BoardController : MonoBehaviour
{

    public int Width;

    public int Height;

    private int[] _clickedXy = {-1, -1};

    private int[] _lastXy = {-1, -1};

    public Creator Creator;

    public GameObjectsController GameObjectsController;

    public int[,] Board;

    public int Score;

    public TextMesh ScoreText;

    public Camera Camera;

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void GenerateLevel()
    {
        Camera.transform.position = new Vector3((Width) / 2f - 2.5f, (Height) / 2f - 2.5f, -10);
        Camera.orthographicSize = (Height - 2)/2f;
        Board = new int[Width, Height];
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Board[x, y] = -1;
            }
        }
        for (int x = 1; x < Width - 1; x++)
        {
            for (int y = 1; y < Height - 1; y++)
            {
                int random = Random.Range(0, 6);
                Board[x, y] = random;
            }
        }
    }

    public void Click(int x, int y)
    {
        _lastXy[0] = _clickedXy[0];
        _lastXy[1] = _clickedXy[1];
        _clickedXy[0] = x;
        _clickedXy[1] = y;
        if (_lastXy[0] != -1)
        {
            if (Mathf.Abs(Mathf.Abs(_lastXy[0] - _clickedXy[0]) + Mathf.Abs(_lastXy[1] - _clickedXy[1])) == 1)
            {
                Creator.MoveAnimation(_lastXy[0], _lastXy[1], _clickedXy[0], _clickedXy[1], true);
            }
            _lastXy[0] = -1;
            _lastXy[1] = -1;
            _clickedXy[0] = -1;
            _clickedXy[1] = -1;
        }
    }

    private List<int[]>[] RowTest(int x, int y)
    {     
        List<int[]> destroyPos = new List<int[]>();
        List<int[]> cutPos = new List<int[]>();
        destroyPos.Add(new[] { x, y });
        int color = Board[x, y];
        if (color > 6)
        {
            cutPos.Add(new[] { x, y });
        }
        //Row Test
        //Идет ли еще ряд вправо?
        bool rightRow = true;
        //Идет ли еще ряд влево?
        bool leftRow = true;
        for (int row = 1; row < 3; row++)
        {
            if (rightRow && IsEqual(color, x + row, y))
            {
                if (Board[x + row, y] > 6)
                {
                    cutPos.Add(new[] { x + row, y });
                }
                destroyPos.Add(new[] { x + row, y });
            }
            else
            {
                rightRow = false;
            }
            if (leftRow && IsEqual(color, x - row, y))
            {
                if (Board[x - row, y] > 6)
                {
                    cutPos.Add(new[] { x - row, y });
                }
                destroyPos.Add(new[] { x - row, y });
            }
            else
            {
                leftRow = false;
            }
        }
        List<int[]>[] destroyAndCutPos = {
            destroyPos,
            cutPos
        };
        return destroyAndCutPos;
    }
    private List<int[]>[] ColumnTest(int x, int y)
    {
        List<int[]> destroyPos = new List<int[]>();
        List<int[]> cutPos = new List<int[]>();
        destroyPos.Add(new []{x, y});
        int color = Board[x, y];
        if (color > 6)
        {
            cutPos.Add(new[] { x, y });
        }
        //Column Test
        //Идет ли еще столбик вверх?        
        bool upColumn = true;
        //Идет ли еще столбик вниз?        
        bool downColumn = true;
        for (int column = 1; column < 3; column++)
        {
            if (upColumn && IsEqual(color, x, y + column))
            {
                if (Board[x, y + column] > 6)
                {
                    cutPos.Add(new[] { x, y + column });
                }
                destroyPos.Add(new[] { x, y + column });
            }
            else
            {
                upColumn = false;
            }
            if (downColumn && IsEqual(color, x, y - column))
            {
                if (Board[x, y - column] > 6)
                {
                    cutPos.Add(new[] { x, y - column });
                }
                destroyPos.Add(new[] { x, y - column });
            }
            else
            {
                downColumn = false;
            }
        }
        List<int[]>[] destroyAndCutPos = {
            destroyPos,
            cutPos 
        };
        return destroyAndCutPos;
    }

    private void Move(int x, int y, int moveX, int moveY)
    {
        int movedColor = Board[x, y];
        Board[x, y] = Board[moveX, moveY];
        Board[moveX, moveY] = movedColor;
    }

    public bool WeHaveMoves()
    {
        for (int x = 1; x < Width - 1; x++)
        {
            for (int y = 1; y < Width - 1; y++)
            {
                if (Board[x, y] == 6)
                {
                    return true;
                }
                for (int moveX = -1; moveX <= 1; moveX++)
                {
                    if (x+moveX < 1 || x+moveX > Width-1)
                    {
                        continue;
                    }
                    Move(x, y, x+moveX, y);
                    if (DestroyTest(x, y) || DestroyTest(x+moveX, y))
                    {
                        Move(x, y, x+moveX, y);
                        return true;
                    }
                    Move(x, y, x+moveX, y);
                }
                for (int moveY = -1; moveY <= 1; moveY++)
                {
                    if (y + moveY < 1 || y + moveY > Height - 1)
                    {
                        continue;
                    }
                    Move(x, y, x, y+moveY);
                    if (DestroyTest(x, y) || DestroyTest(x, y+moveY))
                    {
                        Move(x, y, x, y+moveY);
                        return true;
                    }
                    Move(x, y, x, y+moveY);
                }
            }
        }
        return false;
    }

    public bool DestroyTest(int x, int y)
    {
        if (RowTest(x, y)[0].Count >= 3 || ColumnTest(x, y)[0].Count >= 3)
        {
            return true;
        }
        return false;
    }

    public bool InRowTest(int x, int y)
    {
        int colorOfTestBlock = Board[x, y];
        if (colorOfTestBlock == -1 || colorOfTestBlock == 6)
        {
            return false;
        }
        List<int[]> destroyRowPos = RowTest(x, y)[0];
        int inRow = destroyRowPos.Count;
        List<int[]> cutRowPos = RowTest(x, y)[1];
        List<int[]> destroyColumnPos = ColumnTest(x, y)[0];
        List<int[]> cutColumnPos = ColumnTest(x, y)[1];
        bool willReturn = false;
        if (inRow >= 3)
        {
            foreach (var row in cutRowPos)
            {
                DestroyOneRow(row[0], true);
            }
            DestroyBlocks(destroyRowPos);
            willReturn = true;
        }
        else
        {
            destroyRowPos.Clear();
            destroyRowPos.Add(new[] {x, y});
        }
        if (inRow > 3)
        {
            InRowCreateTest(inRow, colorOfTestBlock, x, y);
        }
        if (colorOfTestBlock > 6)
        {
            cutRowPos.Add(new[] { x, y });
        }
        inRow = destroyColumnPos.Count;
        if (inRow >= 3)
        {
            foreach (var row in cutColumnPos)
            {
                DestroyOneRow(row[1], false);
            }
            DestroyBlocks(destroyColumnPos);
            willReturn = true;
        }
        if (inRow > 3)
        {
            InRowCreateTest(inRow, colorOfTestBlock, x, y);
        }
        return willReturn;
    }

    private void InRowCreateTest(int inRow, int color, int x, int y)
    {
        if (inRow == 5)
        {
            Board[x, y] = 6;
            Creator.CreateBlock(Board[x, y], x, y);
        }
        if (inRow == 4)
        {
            Board[x, y] = color + 7;
            Creator.CreateBlock(Board[x, y], x, y);
        }
    }

    private void DestroyBlocks(List<int[]> destroyBlocks)
    {
        foreach (var destroyPos in destroyBlocks)
        {
            DestroyBlock(destroyPos);
        }
    }

    public void DestroyBlock(int[] destroyPos)
    {
        Destroy(GameObjectsController.GameObjects[destroyPos[0], destroyPos[1]]);
        GameObjectsController.GameObjects[destroyPos[0], destroyPos[1]] = null;
        Board[destroyPos[0], destroyPos[1]] = -1;
        ScoreAdd(10);
    }

    private void GameOver()
    {
        Debug.Log("Game Over!");
    }

    public void Fall()
    {
        Tween t = null;
        float fallSpeed = 0.2f;
        for (int x = 1; x < Width - 1; x++)
        {
            for (int y = 1; y < Height - 1; y++)
            {
                if (Board[x, y] == -1)
                {
                    if (y == Height - 2)
                    {
                        Board[x, y] = Random.Range(0, 6);
                        GameObject createObj = Creator.CreateBlock(Board[x, y], x, y);
                        t =
                            createObj.transform.DOMove(new Vector3(x - 2, y - 2 + 1), fallSpeed)
                                .From()
                                .SetEase(Ease.Linear);
                    }
                    else if (Board[x, y + 1] != -1)
                    {
                        Board[x, y] = Board[x, y + 1];
                        Board[x, y + 1] = -1;
                        GameObject createObj = Creator.CreateBlock(Board[x, y], x, y);
                        Destroy(GameObjectsController.GameObjects[x, y + 1]);
                        GameObjectsController.GameObjects[x, y + 1] = null;
                        t =
                            createObj.transform.DOMove(new Vector3(x - 2, y - 2 + 1), fallSpeed)
                                .From()
                                .SetEase(Ease.Linear);
                    }
                }
            }
        }
        bool weDontHaveHoles = true;
        for (int x = 1; x < Width - 1; x++)
        {
            for (int y = 1; y < Height - 1; y++)
            {
                if (Board[x, y] == -1)
                {
                    weDontHaveHoles = false;
                    break;
                }
            }
        }
        if (weDontHaveHoles)
        {
            t.OnComplete(InRowTestEveryWhere);
            if (!WeHaveMoves())
            {
                GameOver();                
            }
        }
        else
        {
            t.OnComplete(Fall);
        }
    }

    public void InRowTestEveryWhere()
    {
        for (int x = 1; x < Width - 1; x++)
        {
            for (int y = 1; y < Height - 1; y++)
            {
                if (InRowTest(x, y))
                {
                    Fall();
                }
            }
        }
    }

    private bool IsEqual(int color, int x, int y)
    {
        if (color == -1)
        {
            return false;
        }
        if (Board[x, y] == color || Board[x, y] == color + 7 || Board[x, y] == color - 7)
        {
            return true;
        }
        return false;
    }

    public void DestroyOneColor(int color)
    {
        List<int[]> destroyPos = new List<int[]>();
        for (int x = 1; x < Width - 1; x++)
        {
            for (int y = 1; y < Height - 1; y++)
            {
                if (Board[x, y] == color)
                {
                    destroyPos.Add(new[]{x, y});
                }    
            }
        }
        DestroyBlocks(destroyPos);
        Fall();
    }

    public void DestroyOneRow(int xory, bool column)
    {
        var destroyPos = new List<int[]>();
        if (column)
        {
            for (int y = 1; y < Height - 1; y++)
            {
                destroyPos.Add(new[] {xory, y});
            }
            DestroyBlocks(destroyPos);
        }
        else
        {
            for (int x = 1; x < Width - 1; x++)
            {
                destroyPos.Add(new[] {x, xory});
            }
            DestroyBlocks(destroyPos);
        }
    }

    public void ScoreAdd(int add)
    {
        Score += add;
        ScoreText.text = "Score: " + Score;
    }
}

