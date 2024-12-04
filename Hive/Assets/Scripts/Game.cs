using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Game : MonoBehaviour
{
    public GameObject hivepiece;

    private Dictionary<(int x, int y), Stack<GameObject>> positions = new Dictionary<(int, int), Stack<GameObject>>();
    private GameObject[] blackPlayer = new GameObject[9];
    private GameObject[] whitePlayer = new GameObject[9];

    private string currentPlayer = "w";
    private bool gameOver = false;

    void Start()
    {
        whitePlayer = new GameObject[]
        {
            Create("w_queenBee", 28, 1),
            Create("w_ant", 28, 2),
            Create("w_ant", 28, 3),
            Create("w_beetle", 28, 4),
            Create("w_beetle", 28, 5),
            Create("w_spider", 28, 6),
            Create("w_spider", 28, 7),
            Create("w_grasshopper", 28, 8),
            Create("w_grasshopper", 28, 9)
        };

        blackPlayer = new GameObject[]
        {
            Create("b_queenBee", 0, 1),
            Create("b_ant", 0, 2),
            Create("b_ant", 0, 3),
            Create("b_beetle", 0, 4),
            Create("b_beetle", 0, 5),
            Create("b_spider", 0, 6),
            Create("b_spider", 0, 7),
            Create("b_grasshopper", 0, 8),
            Create("b_grasshopper", 0, 9)
        };

        for (int i = 0; i < whitePlayer.Length; i++)
        {
            SetPosition(blackPlayer[i]);
            SetPosition(whitePlayer[i]);
        }
    }

    public GameObject Create(string name, int x, int y)
    {
        GameObject obj = Instantiate(hivepiece, new Vector3(0, 0, -1), Quaternion.identity);
        Hiveman hm = obj.GetComponent<Hiveman>();
        hm.name = name;
        hm.SetXBoard(x);
        hm.SetYBoard(y);
        hm.Activate();
        return obj;
    }

    public void SetPosition(GameObject obj)
    {
        Hiveman hm = obj.GetComponent<Hiveman>();
        var position = (hm.GetXBoard(), hm.GetYBoard());

        if (!positions.ContainsKey(position))
        {
            positions[position] = new Stack<GameObject>();
        }

        positions[position].Push(obj);
        UpdateVisualStack(position);
    }

    public void SetPositionEmpty(int x, int y)
    {
        var position = (x, y);
        if (positions.ContainsKey(position) && positions[position].Count > 0)
        {
            positions[position].Pop();

            if (positions[position].Count == 0)
            {
                positions.Remove(position);
            }
        }
    }

    public GameObject GetPosition(int x, int y)
    {
        var position = (x, y);
        if (positions.ContainsKey(position) && positions[position].Count > 0)
        {
            return positions[position].Peek();
        }
        return null;
    }

    /* public bool CanBeetleMoveOnto(int x, int y)
     {
         var position = (x, y);
         if (positions.ContainsKey(position) && positions[position].Count > 0)
         {
             GameObject topPiece = positions[position].Peek();
             Hiveman topHiveman = topPiece.GetComponent<Hiveman>();

             // Check if the top piece allows stacking (only beetle can jump onto others)
             return topHiveman.name.Contains("beetle");
         }
         return true; // If position is empty, beetle can move there
     }
    */
    public void UpdateVisualStack((int x, int y) position)
    {
        if (positions.ContainsKey(position))
        {
            Stack<GameObject> stack = positions[position];
            int stackSize = stack.Count;

            int index = stack.Count;
            foreach (GameObject obj in stack)
            {
                // Adjust the sorting order dynamically
                SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sortingOrder = index; // Higher index = rendered on top
                }
                index--;
            }
        }
    }


    //is this position blocked due to a beetle
    public bool IsBeetleBlocked(int x, int y, string player)
    {
        var position = (x, y);
        if (positions.ContainsKey(position) && positions[position].Count > 1)
        {
            Stack<GameObject> stack = positions[position];
            GameObject topPiece = stack.Peek();
            Hiveman topHiveman = topPiece.GetComponent<Hiveman>();

            // If top piece is a beetle, the pieces below are blocked
            return topHiveman.name.Contains("beetle") && currentPlayer != player ; //make sure the top beetle is not blocked
        }
        return false;
    }
    public bool IsOnBoard(int x, int y)
    {
        return x >= 0 && y >= 0 && x < 29 && y < 12;
    }
    public bool IsGameOver()
    {
        return gameOver;
    }
    public string GetCurrentPlayer()
    {
        return currentPlayer;
    }
    public void NextTurn()
    {
        currentPlayer = currentPlayer == "w" ? "b" : "w";
    }

    public void Update()
    {
        if (gameOver == true && Input.GetMouseButtonDown(0))
        {
            gameOver = false;
            SceneManager.LoadScene("Game");
        }
    }
}