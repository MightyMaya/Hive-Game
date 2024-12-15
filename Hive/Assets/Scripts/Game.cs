using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System;

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
    public int b_turncount = 0;
    public int w_turncount = 0;
    
    /*public enum GameMode
    {
        HumanVsHuman,
        AIvsHuman,
        AIvsAI
    }*/

    //public GameMode currentMode = GameMode.AIvsHuman;
    public string aiPlayer1 = "b"; // AI player 1
    public string aiPlayer2 = "w"; // AI player 2
    public GameObject aiPlayer;
    public PlayerAI ai;

    void Start()
    {
        whitePlayer = new GameObject[]
        {
            Create("w_queenBee", 30, 0,0),
            Create("w_ant", 30, 1, 0),
            Create("w_ant", 30, 2, 0),
            Create("w_ant", 30, 3, 0),
            Create("w_beetle", 30, 4, 0),
            Create("w_beetle", 30, 5, 0),
            Create("w_spider", 30, 6, 0),
            Create("w_spider", 30, 7, 0),
            Create("w_grasshopper", 30, 8, 0),
            Create("w_grasshopper", 30, 9, 0),
             Create("w_grasshopper", 30, 10, 0)
        };

        blackPlayer = new GameObject[]
        {
            Create("b_queenBee", -2, 0, 0),
            Create("b_ant", -2, 1, 0),
            Create("b_ant", -2, 2, 0),
            Create("b_ant", -2, 3, 0),
            Create("b_beetle", -2, 4, 0),
            Create("b_beetle", -2, 5, 0),
            Create("b_spider", -2, 6, 0),
            Create("b_spider", -2, 7, 0),
            Create("b_grasshopper", -2, 8, 0),
            Create("b_grasshopper", -2, 9, 0),
            Create("b_grasshopper", -2, 10, 0 )
        };

        for (int i = 0; i < whitePlayer.Length; i++)
        {
            SetPosition(blackPlayer[i]);
            SetPosition(whitePlayer[i]);
        }
        
        aiPlayer=GameObject.FindGameObjectWithTag("AiPlayer1");
        ai = aiPlayer.GetComponent<PlayerAI>();

    }

    public GameObject Create(string name, int x, int y, int z)
    {
        GameObject obj = Instantiate(hivepiece, new Vector3(0, 0, -1), Quaternion.identity);
        Hiveman hm = obj.GetComponent<Hiveman>();
        hm.name = name;
        hm.SetXBoard(x);
        hm.SetYBoard(y);
        hm.SetZBoard(z);
        hm.isOnBoard = false;
        hm.Activate();
        return obj;
    }

    public void SetPosition(GameObject obj)
    {

        Hiveman hm = obj.GetComponent<Hiveman>();
        int z = hm.GetZBoard();
        var position = (hm.GetXBoard(), hm.GetYBoard());
       
        // Ignore pieces placed outside the board
        if (!IsOnBoard(hm.GetXBoard(), hm.GetYBoard()))
        {
            Debug.LogWarning("Attempted to set position outside of board boundaries.");
            return;
        }

        //if the position already has a piece
        if (positions.ContainsKey(position))
        {
            //z position of the piece should be incremented
            hm.SetZBoard(++z);
            Debug.Log($"stack position is {z}");
            positions[position].Push(obj);
        }
        else
        {
            //z position of the piece should be zero
            hm.SetZBoard(0);
            positions[position] = new Stack<GameObject>();
            Debug.Log($"stack position is {z}");
            positions[position].Push(obj);
        }

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
    public bool IsBeetleBlocked(int x, int y, int z, string player)
    {
        var position = (x, y);
        if (positions.ContainsKey(position) && positions[position].Count > 1)
        {
            Stack<GameObject> stack = positions[position];
            bool isBlocked = false;

            foreach (var piece in stack)
            {
                Hiveman pieceHiveman = piece.GetComponent<Hiveman>();

                // If we reach the specified z level, stop checking further
                if (pieceHiveman.GetZBoard() == z)
                {
                    break;
                }

                // If there's a beetle above the specified z level, block movement
                if (pieceHiveman.name.Contains("beetle"))
                {
                    isBlocked = true;
                }
            }

            // If the specified piece is the top and a beetle from the same player, allow movement
            GameObject topPiece = stack.Peek();
            Hiveman topHiveman = topPiece.GetComponent<Hiveman>();
            if (topHiveman.name.Contains("beetle") && topHiveman.player == player && topHiveman.GetZBoard() == z)
            {
                isBlocked = false;
            }

            return isBlocked;
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
    public bool IsDraw(int x, int y)
    {
        return isDraw;
    }
    public void SetDraw(bool isDraw_)
    {
        isDraw = isDraw_;
    }

    //function to switch current player
    public void NextTurn()
    {
        
        if (currentPlayer == "w")
        {
            w_turncount++;
            currentPlayer = "b";
        }
        else if (currentPlayer == "b")
        {
           /* if (ai != null )
            {
                currentPlayer = "w";
                b_turncount++;
                Debug.Log("AI Turn");
                //ai.MakeMove("b");

                // List<GameObject> allPieces = ai.GetPlayerPieces();
                Debug.Log("next white turn");
                
            }
            else {*/
                Debug.Log("next white turn");
                currentPlayer = "w";
                b_turncount++;
            //}
            
        }
        // currentPlayer = currentPlayer == "w" ? "b" : "w";
    }

    public bool IsQueenOnBoard(string player)
    {
        GameObject[] playerPieces = player == "w" ? whitePlayer : blackPlayer;

        foreach (GameObject piece in playerPieces)
        {
            Hiveman hiveman = piece.GetComponent<Hiveman>();
            if (hiveman.name == $"{player}_queenBee" && hiveman.isOnBoard)
            {
                return true; // Queen is on the board
            }
        }

        return false; // Queen is not on the board
    }

   

    public void Update()
    {

        // Leave this code commented  , DO NOT Delete it (Fady)
        /*
         *  // New: Check for the draw condition each time a turn ends
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

         */
        
        
        //Update method content on git
        
        //  if (gameOver == true && Input.GetMouseButtonDown(0))
        // {
        //     gameOver = false;
        //     SceneManager.LoadScene("Game");
        // }
        //  
        //---------------------------------------------------------------------------
        // if (gameOver == true && Input.GetMouseButtonDown(0))
        // {
        //     gameOver = false;
        //     SceneManager.LoadScene("Game");
        // }
        // // if (gameOver) return;
        //
        // if (Input.GetKeyDown(KeyCode.A)) // Press "A" to trigger AI move
        // {
        //     this.StartAI();
        // }
        // // Check if it's the AI's turn
        // if (GetCurrentPlayer() == "b") // Assuming "b" is the AI player
        // {
        //     PlayerAI ai = GetComponent<PlayerAI>();
        //     if (ai != null)
        //     {
        //         ai.MakeMove();
        //         NextTurn(); // Pass the turn to the next player
        //     }
        // }
        //----------------------------------------------
        if (gameOver) return;

        if (GameSettings.Instance != null)
        {
            switch (GameSettings.Instance.currentMode)
            {
                case GameSettings.GameMode.HumanVsHuman:
                    Debug.Log("Game Mode: Human Vs. Human");
                    // No AI logic, both players are human
                    break;

                case GameSettings.GameMode.HumanVsAI:
                    Debug.Log("Game Mode: Human Vs. AI");
                    if (GetCurrentPlayer() == aiPlayer1)
                    {
                        Debug.Log("AI vs Human 2");
                        // StartAI(aiPlayer1);

                        List<GameObject> allPieces = ai.GetPlayerPieces();

                        ai.MakeMove(aiPlayer1);
                    }
                        break;

                case GameSettings.GameMode.AIvsAI:
                    Debug.Log("Game Mode: AI Vs. AI");
                    if (GetCurrentPlayer() == aiPlayer1)
                    {
                        ai.MakeMove(aiPlayer1);
                    }
                    else
                    {
                        ai.MakeMove(aiPlayer2);
                    }
                        break;
            }
        }

      /*  if (currentMode == GameMode.HumanVsHuman)
        {
            Debug.Log("Human vs Human");
            // No AI logic, both players are human
            return;
        }

        if (currentMode == GameMode.AIvsHuman)
        {
            Debug.Log("AI vs Human");
            if (GetCurrentPlayer() == aiPlayer1)
            {
                Debug.Log("AI vs Human 2");
                // StartAI(aiPlayer1);
                
                List<GameObject> allPieces = ai.GetPlayerPieces();

                ai.MakeMove(aiPlayer1);
                //NextTurn();
            }
        }
        else if (currentMode == GameMode.AIvsAI)
        {
            Debug.Log("AI vs AI");
            if (GetCurrentPlayer() == aiPlayer1 || GetCurrentPlayer() == aiPlayer2)
            {
                // StartAI(GetCurrentPlayer());
                NextTurn();
            }
        }
      */

    }

    public bool CanPlayerMoveOrPlace(string player)
    {
        bool canMoveOrPlace = false;

        // Check for valid placements of pieces (placement is done in the `IsValidPlacement` method)
        // Assuming a method `IsValidPlacement` already exists in the Hiveman class
        foreach (var piece in FindObjectsOfType<Hiveman>())
        {
            if (GetCurrentPlayer() == player)
            {
                // Check if this piece can move or if a new piece can be placed
                List<Vector2Int> possibleMoves = piece.moveLogic.GetPossibleMoves(piece.GetXBoard(), piece.GetYBoard(), piece.GetZBoard(), player);

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
    public bool CheckForDrawDueRedundentMoves()
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
        // Find the Queen Bees of both players
        GameObject queenWhite = FindQueenBee("w");
        GameObject queenBlack = FindQueenBee("b");

        if (queenWhite != null && queenBlack != null)
        {
            // Check if both queens are surrounded
            Hiveman whiteQueenPiece = queenWhite.GetComponent<Hiveman>();
            Hiveman blackQueenPiece = queenBlack.GetComponent<Hiveman>();

            List<Vector2Int> whiteQueenMoves = whiteQueenPiece.moveLogic.GetPossibleMoves(whiteQueenPiece.GetXBoard(), whiteQueenPiece.GetYBoard(), whiteQueenPiece.GetZBoard(), "w");
            List<Vector2Int> blackQueenMoves = blackQueenPiece.moveLogic.GetPossibleMoves(blackQueenPiece.GetXBoard(), blackQueenPiece.GetYBoard(), blackQueenPiece.GetZBoard(), "b");

            // Check for no valid moves for both queens (surrounded)
            bool whiteQueenSurrounded = whiteQueenMoves.Count == 0;
            bool blackQueenSurrounded = blackQueenMoves.Count == 0;

            if (whiteQueenSurrounded && blackQueenSurrounded)
            {
                // Check if the last move surrounds both queens
                //   if (LastMoveSurroundsBothQueens())
                //    {
                // New: Both queens are surrounded by the same move, game is a draw
                SetDraw(true);
                    EndGameDraw();

                    return;
             //       }
            }

            if (whiteQueenSurrounded)
            {
                EndGame("b");
            }
            else if (blackQueenSurrounded)
            {
                EndGame("w");
            }
        }
        else
        {
            Debug.LogError("One or both queens are missing, cannot determine game end condition.");
        }
    }



    //public void CheckGameEndCondition()
    //{
    //    // Find the Queen Bee of the current player
    //    GameObject queen = FindQueenBee(currentPlayer);

    //    if (queen != null)
    //    {
    //        Hiveman queenPiece = queen.GetComponent<Hiveman>();
    //        List<Vector2Int> validMoves = queenPiece.moveLogic.GetPossibleMoves(queenPiece.GetXBoard(), queenPiece.GetYBoard(), queenPiece.GetZBoard(), currentPlayer);

    //        if (validMoves.Count == 0)
    //        {
    //            EndGame(currentPlayer);
    //        }
    //    }
    //    else
    //    {
    //        Debug.LogError($"No Queen Bee found for player {currentPlayer}!");
    //    }
    //}

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
    public void EndGameDraw()
    {
        Debug.Log("The game is a draw! Both queens were surrounded by the same move.");

        // Handle draw logic (disable input, show draw message, etc.)
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


    //function to simulate moving a piece and check if the movement will diconnect the hive
    public bool DoesPieceDisconnectHive(GameObject piece, int targetX, int targetY)
    {
        // Get the current position of the piece
        int currentX = piece.GetComponent<Hiveman>().GetXBoard();
        int currentY = piece.GetComponent<Hiveman>().GetYBoard();
        var currentPosition = (currentX, currentY);
        var targetPosition = (targetX, targetY);

        // Temporarily remove the piece from its current position
        Stack<GameObject> originalStack = positions[currentPosition];
        originalStack.Pop();
        if (originalStack.Count == 0)
        {
            positions.Remove(currentPosition);
        }
        
        // Check if the hive remains connected after removal
        bool isHiveConnectedAfterRemoval = IsHiveConnected();
        if (!isHiveConnectedAfterRemoval)
        {
            Debug.Log("Move invalid: Removing this piece disconnects the hive.");
            // Restore the piece to its original position
            if (!positions.ContainsKey(currentPosition))
            {
                positions[currentPosition] = new Stack<GameObject>();
            }
            positions[currentPosition].Push(piece);
            return true; // The move is invalid
        }

        // Temporarily add the piece to the target position
        if (!positions.ContainsKey(targetPosition))
        {
            positions[targetPosition] = new Stack<GameObject>();
        }
        positions[targetPosition].Push(piece);

        // Check if the hive remains connected
        bool isHiveConnected = IsHiveConnected();

        // Restore the original state
        // Remove the piece from the target position
        positions[targetPosition].Pop();
        if (positions[targetPosition].Count == 0)
        {
            positions.Remove(targetPosition);
        }

        // Restore the piece to its original position
        if (!positions.ContainsKey(currentPosition))
        {
            positions[currentPosition] = new Stack<GameObject>();
        }
        positions[currentPosition].Push(piece);

        // Return true if moving the piece causes the hive to disconnect
        return !isHiveConnected;
    }


    /// <summary>
    /// Checks if the entire hive is connected.
    /// </summary>
    /// <returns>True if the hive is connected, false otherwise.</returns>
    private bool IsHiveConnected()
    {
        if (positions.Count == 0)
            return true; // No pieces on the board

        // Find a starting position with at least one piece
        (int x, int y)? start = null;
        foreach (var position in positions.Keys)
        {
            if (positions[position].Count > 0)
            {
                start = position;
                break;
            }
        }

        if (start == null)
            return true; // No active pieces, so the hive is trivially connected

        // Set for visited positions
        HashSet<(int x, int y)> visited = new HashSet<(int x, int y)>();
        Queue<(int x, int y)> queue = new Queue<(int x, int y)>();
        queue.Enqueue(start.Value);
        visited.Add(start.Value);

        // Perform BFS
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            foreach (var neighbor in GetAdjacentTiles(new Vector2Int(current.x, current.y)))
            {
                var neighborTuple = (neighbor.x, neighbor.y);

                // Only consider positions with pieces
                if (positions.ContainsKey(neighborTuple) && positions[neighborTuple].Count > 0 && !visited.Contains(neighborTuple))
                {
                    visited.Add(neighborTuple);
                    queue.Enqueue(neighborTuple);
                }
            }
        }

        // Check if all pieces are visited
        foreach (var position in positions.Keys)
        {
            if (positions[position].Count > 0 && !visited.Contains(position))
            {
                return false; // Found a piece that isn't connected
            }
        }

        return true; // All pieces are connected
    }
    //---------------------------------------------------------------
    public Dictionary<(int x, int y), Stack<GameObject>> GetPositions()
    {
        return positions;
    }
    
    public void SetPositions(Dictionary<(int x, int y), Stack<GameObject>> newPositions)
    {
        positions = new Dictionary<(int x, int y), Stack<GameObject>>();
        foreach (var pos in newPositions)
        {
            positions[pos.Key] = new Stack<GameObject>(pos.Value); // Clone the stack
        }
    }
    public void SetCurrentPlayer(string player)
    {
        currentPlayer = player;
    }

    public string GetOpponent(string player)
    {
        return player == "w" ? "b" : "w";
    }
    public List<GameObject> GetPlayerPieces(string player)
    {
        List<GameObject> playerPieces = new List<GameObject>();

        foreach (var stack in positions.Values)
        {
            foreach (var piece in stack)
            {
                Hiveman hiveman = piece.GetComponent<Hiveman>();
                if (hiveman.player == player)
                {
                    playerPieces.Add(piece);
                }
            }
        }

        return playerPieces;
    }
    
    public List<GameObject> GetOpponentPieces(string player)
    {
        string opponent = GetOpponent(player);
        return GetPlayerPieces(opponent);
    }
    
    public GameObject GetUnplacedPiece(string player, string pieceType)
    {
        GameObject[] playerPieces = player == "w" ? whitePlayer : blackPlayer;

        foreach (GameObject piece in playerPieces)
        {
            Hiveman hiveman = piece.GetComponent<Hiveman>();
            if (!hiveman.isOnBoard && hiveman.name.Contains(pieceType))
            {
                return piece;
            }
        }
        return null;
    }

    
    // public void StartAI(string aiPlayer)
    // {
    //     PlayerAI ai = GetComponent<PlayerAI>();
    //     if (ai != null)
    //     {
    //         ai.MakeMove(aiPlayer);
    //         NextTurn(); // Pass the turn to the next player
    //     }
    // }


//----------------------------------------------------------------------------------------------------------------------------
 
    //function to check if a piece can physically slide out of it's position
    public bool IsPositionBlocked(Vector2Int position, Vector2Int targetPosition)
    {
        // Get all adjacent positions
        HashSet<Vector2Int> neighbors = GetAdjacentTiles(position);

        // Find all empty neighbors
        List<Vector2Int> emptyNeighbors = new List<Vector2Int>();
        foreach (var neighbor in neighbors)
        {
            if (GetPosition(neighbor.x, neighbor.y) == null)
                emptyNeighbors.Add(neighbor);
        }

        // If fewer than two empty neighbors, the piece is blocked
        if (emptyNeighbors.Count < 2)
            return true;

        // Calculate the target position based on the movement direction
        //Vector2Int targetPosition = position + moveDirection;

        // Ensure the target position is empty
        if (!emptyNeighbors.Contains(targetPosition))
            return true;

        // Get the connecting sides for the current move direction
        List<Vector2Int> connectingSides = GetConnectingSides(position.x, position.y, targetPosition.x, targetPosition.y);

        // Check if at least one connecting side is empty
        bool validSlide = false;
        foreach (var side in connectingSides)
        {
            if (GetPosition(side.x, side.y) == null)
            {
                validSlide = true;
                break;
            }
        }

        // If no valid slide, position is blocked
        if (!validSlide)
            return true;

        // If all checks pass, the position is not blocked
        return false;
    }

    // Helper function to get connecting sides based on parity-aware movement
    private List<Vector2Int> GetConnectingSides(int fromX, int fromY, int toX, int toY)
    {
        int dx = toX - fromX;
        int dy = toY - fromY;
        bool isEvenRow = fromX % 2 == 0; // Check parity of the current hex

        // Define connecting sides based on movement direction and row parity
        if (dx == 1 && dy == 1) // Moving top-right
            return isEvenRow
                ? new List<Vector2Int> { new Vector2Int(fromX, fromY + 1), new Vector2Int(fromX + 1, fromY) }
                : new List<Vector2Int> { new Vector2Int(fromX, fromY + 1), new Vector2Int(fromX + 1, fromY + 1) };
       else if (dx == -1 && dy == 0) // Moving bottom-left
            return isEvenRow
                ? new List<Vector2Int> { new Vector2Int(fromX - 1, fromY), new Vector2Int(fromX, fromY - 1) }
                : new List<Vector2Int> { new Vector2Int(fromX - 1, fromY + 1), new Vector2Int(fromX, fromY - 1) };
        else if (dx == 0 && dy == 1) // Moving upward
            return new List<Vector2Int> { new Vector2Int(fromX - 1, fromY + 1), new Vector2Int(fromX + 1, fromY + 1) };
        else if (dx == 0 && dy == -1) // Moving downward
            return new List<Vector2Int> { new Vector2Int(fromX - 1, fromY), new Vector2Int(fromX + 1, fromY) };
        else if (dx == 1 && dy == 0) // Moving bottom-right
            return isEvenRow
                ? new List<Vector2Int> { new Vector2Int(fromX, fromY - 1), new Vector2Int(fromX + 1, fromY) }
                : new List<Vector2Int> { new Vector2Int(fromX + 1, fromY + 1), new Vector2Int(fromX, fromY - 1) };
        else //if (dx == -1 && dy == 1) // Moving top-left
            return isEvenRow
                ? new List<Vector2Int> { new Vector2Int(fromX, fromY + 1), new Vector2Int(fromX - 1, fromY) }
                : new List<Vector2Int> { new Vector2Int(fromX, fromY + 1), new Vector2Int(fromX - 1, fromY + 1) };

        //throw new ArgumentException($"Invalid movement from ({fromX}, {fromY}) to ({toX}, {toY})");
    }


   
}
