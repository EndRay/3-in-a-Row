using DG.Tweening;
using UnityEngine;

public class Creator : MonoBehaviour
{

    private int _width;

    private int _height;

    public GameObjectsController GameObjectsController;

    public BoardController BoardController;

    public GameObject Block;

    public GameObject Piece;

    public Sprite[] BlockSprites;

    public Sprite[] PieceColors;


    // Use this for initialization
	void Start ()
	{
	    GenerateLevel();
	}
	
	// Update is called once per frame
	void Update ()
	{


	}

    private void GenerateLevel()
    {
        BoardController.Width += 2;
        BoardController.Height += 2;
        GameObjectsController.GameObjects = new GameObject[BoardController.Width, BoardController.Height];
        BoardController.GenerateLevel();
        _width = BoardController.Width;
        _height = BoardController.Height;
        for (int x = 1; x < _width - 1; x++)
        {
            for (int y = 1; y < _width - 1; y++)
            {
                CreateBlock(BoardController.Board[x, y], x, y);
            }    
        }
        BoardController.InRowTestEveryWhere();
        BoardController.ScoreAdd(-BoardController.Score);
    }

    public GameObject CreateBlock(int color, int x, int y)
    {
        GameObject createObj = 
            Instantiate(Block, new Vector2(x-2, y-2), Quaternion.identity) as GameObject;
        createObj.GetComponent<Block>().X = x;
        createObj.GetComponent<Block>().Y = y;
        GameObjectsController.GameObjects[x, y] = createObj;
        SetBlock(color, x, y);
        return createObj;
    }

    public  void SetBlock(int color, int x, int y)
    {
        GameObject block = GameObjectsController.GameObjects[x, y];
        block.GetComponent<SpriteRenderer>().sprite = BlockSprites[color];
        foreach (Transform children in block.transform)
        {
            children.gameObject.GetComponent<SpriteRenderer>().sprite = PieceColors[color];
        }
    }

    public void MoveAnimation(int x, int y, int moveX, int moveY, bool restart)
    {
        float moveSpeed = 0.3f;
        if (!restart && (BoardController.InRowTest(x, y) | BoardController.InRowTest(moveX, moveY)))
        {
            BoardController.Fall();
            return;
        }
        GameObject[,] gameObjBoard = GameObjectsController.GameObjects;
        int[,] board = BoardController.Board;
        //Star Test
        if (board[x, y] == 6 && board[moveX, moveY] != 6)
        {
            BoardController.DestroyBlock(new[] { x, y });
            BoardController.DestroyOneColor(board[moveX, moveY]);
            return;
        }
        if (board[x, y] != 6 && board[moveX, moveY] == 6)
        {
            BoardController.DestroyBlock(new[] { moveX, moveY });
            BoardController.DestroyOneColor(board[x, y]);
            return;
        }
        //Change Colors
        int blockColor = board[x, y];
        board[x, y] = board[moveX, moveY];
        board[moveX, moveY] = blockColor;
        //Set Colors
        SetBlock(board[x, y], x, y);
        SetBlock(board[moveX, moveY], moveX, moveY);
        //Animation of Move
        gameObjBoard[moveX, moveY].transform.DOMove(new Vector3(x - 2, y - 2), moveSpeed).From();
        Tween t = gameObjBoard[x, y].transform.DOMove(new Vector3(moveX - 2, moveY - 2), moveSpeed).From();
        if (restart)
        {
            t.OnComplete(() => MoveAnimation(x, y, moveX, moveY, false));
        }
    }
}
