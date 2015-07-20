using UnityEngine;

public class Creator : MonoBehaviour
{

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
	void Update () {
	
	}

    private void GenerateLevel()
    {
        BoardController.GenerateLevel();
        for (int x = 2; x < 12; x++)
        {
            for (int y = 2; y < 12; y++)
            {
                CreateBlock(BoardController.Board[x, y], x, y);
            }    
        }
    }

    private void CreateBlock(int color, int x, int y)
    {
        GameObject createObj = 
            Instantiate(Block, new Vector2(x-2, y-2), Quaternion.identity) as GameObject;
        GameObjectsController.GameObjects[x, y] = createObj;
        SetBlock(color, x, y);
    }

    private void SetBlock(int color, int x, int y)
    {
        GameObject block = GameObjectsController.GameObjects[x, y];
        block.GetComponent<SpriteRenderer>().sprite = BlockSprites[color];
        foreach (Transform children in block.transform)
        {
            children.gameObject.GetComponent<SpriteRenderer>().sprite = PieceColors[color];
        }
    }
}
