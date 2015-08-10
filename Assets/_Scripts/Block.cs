using UnityEngine;

public class Block : MonoBehaviour
{
    public BoardController BoardController;

    public int X;

    public int Y;

	// Use this for initialization
	void Start ()
	{
	    BoardController = FindObjectOfType<BoardController>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void OnMouseDown()
    {
        BoardController.Click(X, Y);
    }
}
