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

    public bool isFirstMove = true;

    void Start()
    {
        whitePlayer = new GameObject[]
        {
            Create("w_queenBee", 30, 0),
            Create("w_ant", 30, 1),
            Create("w_ant", 30, 2),
            Create("w_ant", 30, 3),
            Create("w_beetle", 30, 4),
            Create("w_beetle", 30, 5),
            Create("w_spider", 30, 6),
            Create("w_spider", 30, 7),
            Create("w_grasshopper", 30, 8),
            Create("w_grasshopper", 30, 9),
             Create("w_grasshopper", 30, 10)
        };

        blackPlayer = new GameObject[]
        {
            Create("b_queenBee", -2, 0),
            Create("b_ant", -2, 1),
            Create("b_ant", -2, 2),
            Create("b_ant", -2, 3),
            Create("b_beetle", -2, 4),
            Create("b_beetle", -2, 5),
            Create("b_spider", -2, 6),
            Create("b_spider", -2, 7),
            Create("b_grasshopper", -2, 8),
            Create("b_grasshopper", -2, 9),
            Create("b_grasshopper", -2, 10)
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
        hm.isOnBoard = false;
        hm.Activate();
        return obj;
    }

    public void SetPosition(GameObject obj)
    {
        Hiveman hm = obj.GetComponent<Hiveman>();
        var position = (hm.GetXBoard(), hm.GetYBoard());
        /*// For the first move, place the piece at the center of the board
        if (isFirstMove)
        {
            position = (14, 5); // Center of the board
            hm.SetXBoard(14);  
            hm.SetYBoard(5);   
            isFirstMove = false; // After the first move, set the flag to false
        }
        */

        // Ignore pieces placed outside the board
        if (!IsOnBoard(hm.GetXBoard(), hm.GetYBoard()))
        {
            Debug.LogWarning("Attempted to set position outside of board boundaries.");
            return;
        }

        if (!positions.ContainsKey(position))
        {
            positions[position] = new Stack<GameObject>();
        }

        positions[position].Push(obj);
        UpdateVisualStack(position);
    }

    /*
    public void SetPositionEmpty(int x, int y)
    {
        var position = (x, y);
        // Check if the position is tracked in the dictionary
        if (positions.ContainsKey(position)) 
        {
            if(positions[position].Count > 0)
            {
                // Remove the topmost piece from the stack
                positions[position].Pop();
            }

            // If the stack is now empty, remove the key from the dictionary
            if (positions[position].Count == 0)
            {
                positions.Remove(position);
                Stack<GameObject> stack = positions[position];
                Hiveman topPiece = stack.Peek().GetComponent<Hiveman>();

                // Prevent removing a piece that is already on the board
                if (!topPiece.isOnBoard)
                {
                    stack.Pop();
                    if (stack.Count == 0)
                    {
                        positions.Remove(position);
                    }
                }
            }
        }
    }
    */

    public void SetPositionEmpty(int x, int y)
    {
        var position = (x, y);

        if (!IsOnBoard(x, y))
        {
            return;
        }

        if (positions.ContainsKey(position))
        {
            var stack = positions[position];

            if (stack.Count > 0)
            {
                stack.Pop(); // Remove top piece
            }

            if (stack.Count == 0)
            {
                positions.Remove(position); // Clean up empty positions
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
        if (!positions.ContainsKey(position) || positions[position].Count == 0)
        {
            return;
        }
        
        Stack<GameObject> stack = positions[position];

        int index = stack.Count;
        foreach (GameObject obj in stack)
        {
            // Adjust the sorting order dynamically
            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder = index--; // Higher index = rendered on top
            }
          
        }
        
    }


    public List<Vector2Int> GetAdjacentTiles()
    {
        List<Vector2Int> adjacentTiles = new List<Vector2Int>();

        foreach (var pos in positions.Keys)
        {
            int x = pos.Item1;
            int y = pos.Item2;

            adjacentTiles.Add(new Vector2Int(x, y - 1));
            adjacentTiles.Add(new Vector2Int(x, y + 1));
            if (x % 2 == 0)
            {
                // Add all six adjacent positions in a hexagonal grid
                adjacentTiles.Add(new Vector2Int(x - 1, y));
                adjacentTiles.Add(new Vector2Int(x + 1, y));

                adjacentTiles.Add(new Vector2Int(x - 1, y + 1));
                adjacentTiles.Add(new Vector2Int(x + 1, y + 1));

            }
            else
            {
                adjacentTiles.Add(new Vector2Int(x - 1, y));
                adjacentTiles.Add(new Vector2Int(x + 1, y));

                adjacentTiles.Add(new Vector2Int(x - 1, y - 1));
                adjacentTiles.Add(new Vector2Int(x + 1, y - 1));
            }
        }

        return adjacentTiles;
    }

    public bool IsValidPlacement(int x, int y)
    {
        return IsOnBoard(x, y) && GetPosition(x, y) == null;
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