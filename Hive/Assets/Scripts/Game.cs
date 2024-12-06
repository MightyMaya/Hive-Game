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
    public int moveCount = 0;


    // New variables to store the last two moves of each player
    private List<(string pieceName, int x, int y)> whitePlayerMoves = new List<(string pieceName, int x, int y)>();
    private List<(string pieceName, int x, int y)> blackPlayerMoves = new List<(string pieceName, int x, int y)>();

    private const int MAX_MOVES_HISTORY = 2; // New: Track only the last two moves for each player

    // New variable to track if the game has been declared a draw
    private bool isDraw = false;



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


    //Handling Adjacent movements
    //helper fn
    private HashSet<Vector2Int> GetAdjacentTiles(Vector2Int position)
    {
        HashSet<Vector2Int> adjacentTiles = new HashSet<Vector2Int>();

        int x = position.x;
        int y = position.y;

        // Add all adjacent positions directly to the HashSet
        adjacentTiles.Add(new Vector2Int(x, y - 1));
        adjacentTiles.Add(new Vector2Int(x, y + 1));
        adjacentTiles.Add(new Vector2Int(x - 1, y));
        adjacentTiles.Add(new Vector2Int(x + 1, y));

        if (x % 2 == 0)
        {
            adjacentTiles.Add(new Vector2Int(x - 1, y + 1));
            adjacentTiles.Add(new Vector2Int(x + 1, y + 1));
        }
        else
        {
            adjacentTiles.Add(new Vector2Int(x - 1, y - 1));
            adjacentTiles.Add(new Vector2Int(x + 1, y - 1));
        }

        return adjacentTiles;
    }

    //Get Adjacent to all pieces
    public HashSet<Vector2Int> GetTilesAdjacentToAllPieces()
    {
        HashSet<Vector2Int> allAdjacentTiles = new HashSet<Vector2Int>();

        foreach (var pos in positions.Keys)
        {
            HashSet<Vector2Int> adjacentTiles = GetAdjacentTiles(new Vector2Int(pos.Item1, pos.Item2));

            foreach (var tile in adjacentTiles)
            {
                if (!positions.ContainsKey((tile.x, tile.y)) && IsOnBoard(tile.x, tile.y)) // Exclude occupied tiles and not on board tiles
                {
                    allAdjacentTiles.Add(tile);
                }
            }
        }

        return allAdjacentTiles;
    }


    public HashSet<Vector2Int> GetAdjacentTilesForCurrentPlayer()
    {
        HashSet<Vector2Int> adjacentTiles = new HashSet<Vector2Int>();
        HashSet<Vector2Int> opponentAdjacentTiles = new HashSet<Vector2Int>();

        // Get adjacent tiles for the current player's pieces
        foreach (var position in positions.Keys)
        {
            var stack = positions[position];
            if (stack.Count > 0)
            {
                GameObject topPiece = stack.Peek();
                Hiveman piece = topPiece.GetComponent<Hiveman>();

                if (piece.player == currentPlayer)
                {
                    var adjacent = GetAdjacentTiles(new Vector2Int(position.Item1, position.Item2));
                    foreach (var tile in adjacent)
                    {
                        if (IsOnBoard(tile.x, tile.y) && GetPosition(tile.x, tile.y) == null)
                        {
                            adjacentTiles.Add(tile);
                        }
                    }
                }
                else // Opponent's pieces
                {
                    var adjacent = GetAdjacentTiles(new Vector2Int(position.Item1, position.Item2));
                    foreach (var tile in adjacent)
                    {
                        if (IsOnBoard(tile.x, tile.y))
                        {
                            opponentAdjacentTiles.Add(tile);
                        }
                    }
                }
            }
        }

        // Remove tiles adjacent to opponent pieces
        adjacentTiles.ExceptWith(opponentAdjacentTiles);

        return adjacentTiles;
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
        // New: Check for the draw condition each time a turn ends
        if (CheckForDraw())
        {
            isDraw = true;
            Debug.Log("The game is a draw (Fady).");
            // Optionally, trigger game over or stop further moves
            return; // Stop further game updates
        }



        // Check if the player has any valid moves or piece placements
        if (CanPlayerMoveOrPlace(currentPlayer) == false)
        {
            // If no valid moves are available, pass the turn to the opponent
            NextTurn(); // NEW: Pass the turn to the opponent
        }


        if (gameOver == true && Input.GetMouseButtonDown(0))
        {
            gameOver = false;
            SceneManager.LoadScene("Game");
        }
    }
  
    private bool CanPlayerMoveOrPlace(string player)
    {
        bool canMoveOrPlace = false;

        // Check for valid placements of pieces (placement is done in the `IsValidPlacement` method)
        // Assuming a method `IsValidPlacement` already exists in the Hiveman class
        foreach (var piece in FindObjectsOfType<Hiveman>())
        {
            if (GetCurrentPlayer() == player)
            {
                // Check if this piece can move or if a new piece can be placed
                List<Vector2Int> possibleMoves = piece.moveLogic.GetPossibleMoves(piece.GetXBoard(), piece.GetYBoard(), player);

                // If there are any valid moves or placements
                if (possibleMoves.Count > 0 /*||  piece.IsValidPlacement(piece.GetXBoard(), piece.GetYBoard())*/)
                {
                    canMoveOrPlace = true;
                    break; // No need to check further if one valid move/placement is found
                }
            }
        }

        return canMoveOrPlace;
    }

    // New: This method checks if the game is in a draw condition
    private bool CheckForDraw()
    {
        // Check if both players have repeated their last two moves
        if (whitePlayerMoves.Count >= 2 && blackPlayerMoves.Count >= 2)
        {
            var whiteLastMoves = whitePlayerMoves.GetRange(whitePlayerMoves.Count - 2, 2);
            var blackLastMoves = blackPlayerMoves.GetRange(blackPlayerMoves.Count - 2, 2);

            // Check if both players have repeated the same two moves
            if (whiteLastMoves[0].Equals(whiteLastMoves[1]) && blackLastMoves[0].Equals(blackLastMoves[1]))
            {
                // If both players are stuck in a loop, declare a draw
                return true;
            }
        }
        return false;
    }

    // New: Method to record a player's move (for both white and black)
    public void RecordPlayerMove(string player, string pieceName, int x, int y)
    {
        // New: Store the last two moves for each player
        List<(string pieceName, int x, int y)> playerMoves = player == "w" ? whitePlayerMoves : blackPlayerMoves;
        playerMoves.Add((pieceName, x, y));

        // Limit the list to the last 2 moves
        if (playerMoves.Count > MAX_MOVES_HISTORY)
        {
            playerMoves.RemoveAt(0);
        }
    }

    public void CheckGameEndCondition()
    {
        // Find the Queen Bee of the current player
        GameObject queen = FindQueenBee(currentPlayer);

        if (queen != null)
        {
            Hiveman queenPiece = queen.GetComponent<Hiveman>();
            List<Vector2Int> validMoves = queenPiece.moveLogic.GetPossibleMoves(queenPiece.GetXBoard(), queenPiece.GetYBoard(), currentPlayer);

            if (validMoves.Count == 0)
            {
                EndGame(currentPlayer);
            }
        }
        else
        {
            Debug.LogError($"No Queen Bee found for player {currentPlayer}!");
        }
    }

    private GameObject FindQueenBee(string player)
    {
        // Find all pieces on the board
        GameObject[] pieces = GameObject.FindGameObjectsWithTag("Piece");

        foreach (GameObject piece in pieces)
        {
            if (piece.name == $"{player}_queenBee")
            {
                return piece;
            }
        }

        return null; // Queen Bee not found
    }

    private void EndGame(string losingPlayer)
    {
        string winner = losingPlayer == "b" ? "White" : "Black";
        Debug.Log($"Congratulations {winner}, you win!");

        // Handle game-ending logic (disable input, display winner, etc.)
        gameOver = true;
    }
    /*
    public void ProcessTurnEnd()
    {
        CheckGameEndCondition();

        if (gameOver)
        {
            NextTurn();
        }
    }*/
    /*
    public void HandlePieceMove(GameObject piece, int targetX, int targetY)
    {
        // Move the piece
        piece.GetComponent<Hiveman>().MovePiece(targetX, targetY);

        // Record the move and check if the game ends
        ProcessTurnEnd();
    }

    */
}