using UnityEngine;

public class BoardController : MonoBehaviour
{

    public int[,] Board = new int[12,12];

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void GenerateLevel()
    {
        for (int x = 2; x < 12; x++)
        {
            for (int y = 2; y < 12; y++)
            {
                int random = Random.Range(0, 6);
                Board[x, y] = random;
            }    
        }
    }

    private void MoveBlock()
    {
    }
}
