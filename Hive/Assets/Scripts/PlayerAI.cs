using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DefaultNamespace;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;
using static Hiveman;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.UI.Image;
using Debug = UnityEngine.Debug;

public class PlayerAI : MonoBehaviour
{
    public GameObject controller;

    private GameObject b_queen;

    private string aiPlayer = "b";

    private Game gamesc;

    private Hiveman b_queen_controller;

    private MovePlate b_queen_moveplate;

    // Flag to ensure OnMouseUp is called only once
    private bool actionTriggered = false;

    //list to store all the black pieces
    public List<GameObject> b_pieces;

    //list to store all the move scripts
    private List<IMoveLogic> moveScripts;


    public GameSettings.Difficulty aiDifficulty = GameSettings.Instance.aiDifficulty1;


    private void Start()
    {
        // Get a reference to game controller
        controller = GameObject.FindWithTag("GameController");
        gamesc = controller.GetComponent<Game>();

        moveScripts = new List<IMoveLogic>();

        // Find all GameObjects with the tag "Piece"
        GameObject[] allPieces = GameObject.FindGameObjectsWithTag("Piece");

        // Filter pieces whose names start with "b"
        b_pieces = new List<GameObject>();
        foreach (GameObject piece in allPieces)
        {
            if (piece.name.StartsWith("b"))
            {
                b_pieces.Add(piece);
            }
        }

        foreach (GameObject piece in b_pieces)
        {
            switch (piece.name)
            {
                case "b_queenBee":
                    moveScripts.Add(piece.GetComponent<QueenBeeMoves>());
                    break;
                case "b_ant":
                    moveScripts.Add(piece.GetComponent<AntMoves>());
                    break;
                case "b_beetle":
                    moveScripts.Add(piece.GetComponent<BeetleMoves>());
                    break;
                case "b_grasshopper":
                    moveScripts.Add(piece.GetComponent<GrassMoves>());
                    break;
                case "b_spider":
                    moveScripts.Add(piece.GetComponent<SpiderMoves>());
                    break;
            }
        }

        /*// Ensure there's at least one "b" piece
        if (b_pieces.Count > 0)
        {
            Debug.Log($"Found {b_pieces.Count} black  pieces");
        }*/

        // Ensure there's at least one move script
        if (b_pieces.Count > 0)
        {
            Debug.Log($"Found {moveScripts.Count} move scripts");
        }
    }
    // Deep copy of gamesc.positions before simulating the move
    Dictionary<(int x, int y), Stack<GameObject>> DeepCopyPositions(Dictionary<(int x, int y), Stack<GameObject>> original)
    {
        var copy = new Dictionary<(int x, int y), Stack<GameObject>>();

        foreach (var entry in original)
        {
            // Create a new stack and copy all GameObjects
            Stack<GameObject> copiedStack = new Stack<GameObject>(entry.Value.Reverse());
            copy[entry.Key] = copiedStack;
        }

        return copy;
    }
    public void MakeMove(string aiPlayer)
    {
        controller = GameObject.FindWithTag("GameController");
        gamesc = controller.GetComponent<Game>();
        StartCoroutine(MakeMoveCoroutine(aiPlayer));
    }

    private IEnumerator MakeMoveCoroutine(string aiPlayer)
    {
        int maxDepth = (int)aiDifficulty; // Use depth based on difficulty
        GameObject bestPieceToMove = null, bestPieceToPlace = null;
        Vector2Int bestMove = Vector2Int.zero, bestTileToPlace = Vector2Int.zero;
        int bestMoveScore = int.MinValue, bestPlacementScore = int.MinValue;
        Debug.Log("AI is making a move...");

        // Loop through all pieces
        foreach (GameObject piece in GetPlayerPieces())
        {
            Debug.Log($"Evaluating move for {piece.name}");
            Hiveman hiveman = piece.GetComponent<Hiveman>();
            List<Vector2Int> possibleMoves = GetPossibleMoves(hiveman);
            int moveScore = 0;
            if (hiveman.isOnBoard)
            {
                foreach (Vector2Int move in possibleMoves)
                {
                    Debug.Log($"Evaluating move for in board {piece.name} onboard to ({move.x}, {move.y})");
                    Dictionary<(int x, int y), Stack<GameObject>> temp_positions = DeepCopyPositions(gamesc.positions);
                    Vector2Int original = SimulatePlacement(piece, move); // Simulate the move
                    Debug.Log("in min max");
                    moveScore = Minimax(1, true, int.MinValue, int.MaxValue); // Evaluate move
                    Debug.Log("out of min max");
                    RestoreMove(piece, move, original);
                    gamesc.positions = temp_positions;


                    if (moveScore > bestMoveScore)
                    {
                        bestMoveScore = moveScore;
                        bestPieceToMove = piece;
                        bestMove = move;
                    }
                }
            }
            else
            {
                foreach (Vector2Int move in possibleMoves)
                {
                    Debug.Log($"Evaluating move for out of board {piece.name} onboard to ({move.x}, {move.y})");
                    Dictionary<(int x, int y), Stack<GameObject>> temp_positions = DeepCopyPositions(gamesc.positions);
                    Vector2Int original = SimulatePlacement2(piece, move); // Simulate the move
                    Debug.Log("in min max");
                    moveScore = Minimax(1, true, int.MinValue, int.MaxValue); // Evaluate move
                    Debug.Log("out of min max");
                    RestoreMove2(piece, move, original);
                    gamesc.positions = temp_positions;

                    if (moveScore > bestMoveScore)
                    {
                        bestMoveScore = moveScore;
                        bestPieceToMove = piece;
                        bestMove = move;
                    }
                }
            }
        }
        Hiveman besthiveman = bestPieceToMove.GetComponent<Hiveman>();
        if (bestPlacementScore >= bestMoveScore && bestPieceToPlace != null)
        {
            // Place the piece with the best placement score
            if (besthiveman.isOnBoard) MovePiece(bestPieceToPlace, bestTileToPlace);
            else {
                MovePiece2(bestPieceToPlace, bestTileToPlace);
            }
            yield return new WaitForSeconds(3f); // Wait for 1 second
            Debug.Log($"{aiPlayer} placed {bestPieceToPlace.name} at ({bestTileToPlace.x}, {bestTileToPlace.y}).");
        }
        else if (bestPieceToMove != null)
        {
            // Move the piece with the best move score
            if (besthiveman.isOnBoard) MovePiece(bestPieceToMove, bestMove);
            else {
                MovePiece2(bestPieceToMove, bestMove);

            }

            yield return new WaitForSeconds(3f); // Wait for 1 second
            Debug.Log($"{aiPlayer} moved {bestPieceToMove.name} to ({bestMove.x}, {bestMove.y}).");

        }
        else
        {
            Debug.Log("No valid moves or placements available.");
        }
    }


    public List<GameObject> GetPlayerPieces()
    {
        GameObject[] allPieces = GameObject.FindGameObjectsWithTag("Piece");
        List<GameObject> playerPieces = new List<GameObject>();
        foreach (GameObject piece in allPieces)
        {
            if (piece.GetComponent<Hiveman>().player == aiPlayer)
            {
                playerPieces.Add(piece);
            }
        }

        return playerPieces;
    }

    private int EvaluatePiecePriority(string pieceName)
    {
        if (pieceName.Contains("ant")) return 30; // High mobility
        if (pieceName.Contains("grasshopper")) return 20; // Jumping capability
        if (pieceName.Contains("beetle")) return 20; // Defensive
        if (pieceName.Contains("spider")) return 10; // Strategic movement
        if (pieceName.Contains("queenBee"))
        {
                int queen=(gamesc.moveCount < 8 && !gamesc.IsQueenOnBoard(gamesc.GetCurrentPlayer())) ?  50 :  5; // Only if mandatory
                return queen;
        }
        return 0; // Default for unrecognized pieces
    }



    private int EvaluateOpponentProximity(Vector2Int position, string player)
    {
        string opponent = gamesc.GetOpponent(player);

        HashSet<Vector2Int> adjacentTiles = gamesc.GetAdjacentTiles(position);
        foreach (Vector2Int tile in adjacentTiles)
        {
            GameObject piece = gamesc.GetPosition(tile.x, tile.y);
            Hiveman hiveman = piece.GetComponent<Hiveman>();
            if (piece != null && hiveman.player == opponent)
            {
                if(hiveman.GetName()=="w_queenBee" || hiveman.GetName()=="b_queenBee")
                    return 10; // Reward for proximity to opponent pieces
            }
        }

        return 0;
    }
    public int EvaluateQueenSafety()    //GameObject queen, bool isAIPlayer
    {
        // Hiveman hiveman = queen.GetComponent<Hiveman>();
        // if (hiveman == null)
        // {
        //     Debug.LogError("Queen is null or missing Hiveman component!");
        //     return 0;
        // }
        //
        // // Get the current player's and opponent's piece type
        // string playerType = isAIPlayer ? "friendly" : "opponent";
        // string opponentType = isAIPlayer ? "opponent" : "friendly";

        // Calculate the number of surrounding opponent and friendly pieces
        int mySurrounding = GetSurroundingPiecesCount(gamesc.GetCurrentPlayer());
        int opponentSurrounding = GetSurroundingPiecesCount(gamesc.GetOpponent(gamesc.GetCurrentPlayer()));

        // Calculate the queen's mobility
        // int queenMobility = GetPossibleMoves(hiveman).Count;

        // Apply weights to calculate the heuristic value
        int heuristicValue = -(mySurrounding) // Weight for mySurrounding
                             + (opponentSurrounding); // Weight for opponentSurrounding
       
        // Make heuristicValue absolute
        if (heuristicValue < 0)
        {
            heuristicValue = -heuristicValue;
        }

        Debug.Log($"Queen Safety Score : {heuristicValue}");//(isAIPlayer={isAIPlayer})
        return heuristicValue;
    }

    //private Dictionary<(int x, int y), Stack<GameObject>> positions = new Dictionary<(int, int), Stack<GameObject>>();
    

// Function to get the number of surrounding pieces of a specific type (friendly or opponent)
public int GetSurroundingPiecesCount(string player) //GameObject piece, string pieceType
{
    GameObject piece =gamesc.GetQueenPiece(player);
    
    if(piece == null) return 0;

    // Get the position of the piece using GetXBoard() and GetYBoard()
    int x = piece.GetComponent<Hiveman>().GetXBoard();
    int y = piece.GetComponent<Hiveman>().GetYBoard();

    // Initialize the count
    int surroundingCount = 0;

    // Get the adjacent tiles
    List<Vector2Int> adjacentTiles = gamesc.GetAdjacentTiles(new Vector2Int(x, y)).ToList();

    // Loop through the adjacent tiles and check the type of pieces on them
    foreach (var tile in adjacentTiles)
    {
        var position = (tile.x, tile.y);
        if (gamesc.GetPosition(tile.x, tile.y) != null)
        {
            surroundingCount++;
        }

        // Check if there are any pieces on the adjacent tile
        // if (positions.ContainsKey(position) && positions[position].Count > 0)
        // {
        //     // Check the piece type on the adjacent tile
        //     GameObject adjacentPiece = positions[position].Peek();  // Get the top piece
        //     string currentPieceType = adjacentPiece.GetComponent<Hiveman>().GetPieceType(adjacentPiece,gamesc.GetCurrentPlayer());  // Assuming GetPieceType() returns "friendly" or "opponent"
        //
        //     // Compare the piece type to the specified one
        //     if (currentPieceType.Equals(pieceType))
        //     {
        //         surroundingCount++;
        //     }
        // }
    }

    return surroundingCount;
}
    private List<Vector2Int> GetPossibleMoves(Hiveman hiveman)
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();
        Debug.Log($"Fetching possible moves for {hiveman.name}");

        if (!hiveman.isOnBoard)
        {
            if (gamesc.moveCount == 1)
            {
               // Debug.Log("Second move of the game: Highlighting tiles adjacent to all pieces.");
                possibleMoves = gamesc.GetTilesAdjacentToAllPieces().ToList();
            }
            else
            {
                Debug.Log("Subsequent move: Highlighting tiles adjacent to the passed hiveman player's pieces.");
                possibleMoves = gamesc.GetAdjacentTilesForPlayer(hiveman.player).ToList();
            }
        }
        else
        {
            if (hiveman.moveLogic == null)
            {
                Debug.LogError($"{hiveman.name} does not have a moveLogic assigned!");
                return possibleMoves;
            }

            if (gamesc.moveCount == 4 && !gamesc.IsQueenOnBoard(hiveman.player))
            {
                return possibleMoves;
                Debug.LogWarning($"Queen is not on the board yet for the passed player {hiveman.player}. No moves allowed.");

            }
            else if (gamesc.IsQueenOnBoard(hiveman.player))
            {
                Debug.LogWarning($"Queen is not on the board yet for the passed player {hiveman.player}. No moves allowed.");
                Debug.Log($"Fetching moves for ai {hiveman.name} on the board.");
                possibleMoves = hiveman.moveLogic.GetPossibleMoves(hiveman.GetXBoard(), hiveman.GetYBoard(),
                    hiveman.GetZBoard(), hiveman.player);
            }
        }

        Debug.Log($"Possible moves for ai {hiveman.name}: {possibleMoves.Count} moves found.");
        return possibleMoves;
    }


    private Vector2Int SimulateMove(GameObject piece, Vector2Int target)
    {
        Hiveman hiveman = piece.GetComponent<Hiveman>();
        if (hiveman == null) return new Vector2Int();
        // Save the current position to restore later if needed
        int originalX = hiveman.GetXBoard();
        int originalY = hiveman.GetYBoard();
        // Remove the piece from its current position
        gamesc.SetPositionEmpty(originalX, originalY);
        // Move the piece to the target position
        hiveman.SetXBoard(target.x);
        hiveman.SetYBoard(target.y);
        // Update the game board with the new position
        gamesc.SetPosition(piece);
        Debug.Log($"{hiveman.name} moved to ({target.x}, {target.y})");
        return new Vector2Int(originalX, originalY);
    }

    private void RestoreMove(GameObject piece, Vector2Int target, Vector2Int original)
    {
        Controller = GameObject.FindGameObjectWithTag("GameController");
        Game gamesc = Controller.GetComponent<Game>();
        gamesc.SetPositionEmpty(target.x, target.y);
        Hiveman hivesc = piece.GetComponent<Hiveman>();
        //set new position of the piece to the clicked position
        hivesc.SetXBoard(original.x);
        hivesc.SetYBoard(original.y);
        hivesc.SetCoords();
        gamesc.SetPosition(piece);
    }

    private void RestoreMove2(GameObject piece, Vector2Int target, Vector2Int original)
    {
        Controller = GameObject.FindGameObjectWithTag("GameController");
        Game gamesc = Controller.GetComponent<Game>();
        Hiveman hivesc = piece.GetComponent<Hiveman>();
        hivesc.isOnBoard = !hivesc.isOnBoard;
        //set new position of the piece to the clicked position
        hivesc.SetXBoard(original.x);
        hivesc.SetYBoard(original.y);
        hivesc.SetCoords();
        gamesc.SetPosition(piece);
    }
    public Vector2Int SimulatePlacement(GameObject piece, Vector2Int position)
    {
        int currentX = piece.GetComponent<Hiveman>().GetXBoard();
        int currentY = piece.GetComponent<Hiveman>().GetYBoard();
        Controller = GameObject.FindGameObjectWithTag("GameController");
        Game gamesc = Controller.GetComponent<Game>();
        gamesc.SetPositionEmpty(currentX, currentY);
        Hiveman hivesc = piece.GetComponent<Hiveman>();
        //set new position of the piece to the clicked position
        hivesc.SetXBoard(position.x);
        hivesc.SetYBoard(position.y);
        hivesc.SetCoords();
        gamesc.SetPosition(piece);
        return new Vector2Int(currentX, currentY);
    }

    public Vector2Int SimulatePlacement2(GameObject piece, Vector2Int position)
    {

        int currentX = piece.GetComponent<Hiveman>().GetXBoard();
        int currentY = piece.GetComponent<Hiveman>().GetYBoard();
        Controller = GameObject.FindGameObjectWithTag("GameController");
        Game gamesc = Controller.GetComponent<Game>();
        Hiveman hivesc = piece.GetComponent<Hiveman>();
        hivesc.isOnBoard = !hivesc.isOnBoard;
        //set new position of the piece to the clicked position
        hivesc.SetXBoard(position.x);
        hivesc.SetYBoard(position.y);
        hivesc.SetCoords();
        gamesc.SetPosition(piece);
        return new Vector2Int(currentX, currentY);
    }

    public GameObject Controller;

    //reference to the piece that created the moveplate
    public void MovePiece2(GameObject piece, Vector2Int targetPosition)
    {
        Controller = GameObject.FindGameObjectWithTag("GameController");
        Game gamesc = Controller.GetComponent<Game>();
        Hiveman hivesc = piece.GetComponent<Hiveman>();
        Debug.Log("movepiece2");

        if (!hivesc.hiveBreak)
        {
            // Set old position of the piece to be empty
            //gamesc.SetPositionEmpty(hivesc.GetXBoard(), hivesc.GetYBoard());
            // Set new position of the piece to the specified target position
            hivesc.SetXBoard(targetPosition.x);
            hivesc.SetYBoard(targetPosition.y);
            hivesc.SetCoords();

            // Update the game controller with the piece's new position
            gamesc.SetPosition(piece);
            // If this was the first move, toggle the flag
            if (gamesc.isFirstMove)
            {
                gamesc.isFirstMove = false;
                Debug.Log("First move completed.");
            }
            gamesc.moveCount++;  // Increment the move counter

            if (!hivesc.isOnBoard)
            {
                hivesc.isOnBoard = true;
                Debug.Log($"{piece.name} is now placed on the board.");
            }

            // Record the move
            gamesc.RecordPlayerMove(gamesc.GetCurrentPlayer(), hivesc.name, targetPosition.x, targetPosition.y);

            // Switch the player
            gamesc.NextTurn();

            // Destroy any move plates made
            //hivesc.DestroyMovePlates();

            gamesc.CheckGameEndCondition();

            // Check for draw condition after the move
            if (gamesc.CheckForDrawDueRedundentMoves())
            {
                gamesc.SetDraw(true);
                gamesc.EndGameDraw();
                Debug.Log("The game is a draw (Fady).");
                // Optionally, trigger game over or stop further moves
                return; // Stop further game updates
            }
            // Check if the player has any valid moves or piece placements
            if (gamesc.CanPlayerMoveOrPlace(gamesc.GetCurrentPlayer()) == false)
            {
                // If no valid moves are available, pass the turn to the opponent
                gamesc.NextTurn(); // NEW: Pass the turn to the opponent
            }
        }
        else
        {
            Debug.Log($"Move rejected: {piece.name} would break the hive.");
        }
    }
    public void MovePiece(GameObject piece, Vector2Int targetPosition)
    {
        Controller = GameObject.FindGameObjectWithTag("GameController");
        Game gamesc = Controller.GetComponent<Game>();
        Hiveman hivesc = piece.GetComponent<Hiveman>();
        Debug.Log("movepiece1");
        if (!hivesc.hiveBreak)
        {
            // Set old position of the piece to be empty
            gamesc.SetPositionEmpty(hivesc.GetXBoard(), hivesc.GetYBoard());

            // Set new position of the piece to the specified target position
            hivesc.SetXBoard(targetPosition.x);
            hivesc.SetYBoard(targetPosition.y);
            hivesc.SetCoords();

            // Update the game controller with the piece's new position
            gamesc.SetPosition(piece);

            // If this was the first move, toggle the flag
            if (gamesc.isFirstMove)
            {
                gamesc.isFirstMove = false;
                Debug.Log("First move completed.");
            }
            gamesc.moveCount++;  // Increment the move counter

            if (!hivesc.isOnBoard)
            {
                hivesc.isOnBoard = true;
                Debug.Log($"{piece.name} is now placed on the board.");
            }

            // Record the move
            gamesc.RecordPlayerMove(gamesc.GetCurrentPlayer(), hivesc.name, targetPosition.x, targetPosition.y);

            // Switch the player
            gamesc.NextTurn();

            // Destroy any move plates made
            //hivesc.DestroyMovePlates();

            gamesc.CheckGameEndCondition();

            // Check for draw condition after the move
            if (gamesc.CheckForDrawDueRedundentMoves())
            {
                gamesc.SetDraw(true);
                gamesc.EndGameDraw();
                Debug.Log("The game is a draw (Fady).");
                // Optionally, trigger game over or stop further moves
                return; // Stop further game updates
            }
            // Check if the player has any valid moves or piece placements
            if (gamesc.CanPlayerMoveOrPlace(gamesc.GetCurrentPlayer()) == false)
            {
                // If no valid moves are available, pass the turn to the opponent
                gamesc.NextTurn(); // NEW: Pass the turn to the opponent
            }
        }
        else
        {
            Debug.Log($"Move rejected: {piece.name} would break the hive.");
        }
    }

    private void RestoreGameState(GameState state, GameObject piece)
    {
        Hiveman hiveman = piece.GetComponent<Hiveman>();
        hiveman.isOnBoard = !hiveman.isOnBoard;
        state.Restore(gamesc, aiPlayer);
    }

    private int EvaluateGameState()
    {
        // Evaluate using heuristics
        //int mobilityScore = EvaluatePieceMobility(); // - EvaluatePieceMobility(gamesc.GetOpponent("b"));
        //Debug.Log($"{mobilityScore} total mobility score");
        int queenSafetyScore = EvaluateQueenSafety();
        Debug.Log($"{queenSafetyScore} total queen safety score");
        int opponentQueenSurround = EvaluateOpponentQueenSurround();
        int score;
        Controller = GameObject.FindGameObjectWithTag("GameController");
        Game gamesc = Controller.GetComponent<Game>();
        if (gamesc.IsQueenOnBoard(gamesc.GetCurrentPlayer())) score = 10 * opponentQueenSurround - queenSafetyScore;//+ mobilityScore;
        else score = 0;
        if (score < 0) return -score;
        else return score;
    }

        private int EvaluateOpponentQueenSurround()
    {
        int surroundCount = 0;
        GameObject opponentQueen = gamesc.GetQueenPiece(gamesc.GetOpponent(aiPlayer));

         if (opponentQueen != null)
        {
            Hiveman hiveman = opponentQueen.GetComponent<Hiveman>();
            List<Vector2Int> adjacentTiles = gamesc.GetAdjacentTiles(new Vector2Int(hiveman.GetXBoard(), hiveman.GetYBoard())).ToList();

            foreach (Vector2Int tile in adjacentTiles)
            {
                if (gamesc.GetPosition(tile.x, tile.y) != null)
                {
                    surroundCount++;
                }
            }
        }
        return surroundCount; // More pieces around the queen means higher score
    }
    

    public int EvaluatePieceMobility() //string aiPlayer
    {
        int mobilityScore = 0;
        foreach (GameObject piece in GetPlayerPieces())
        {
            Hiveman hiveman = piece.GetComponent<Hiveman>();
            if (hiveman == null || !hiveman.isOnBoard) {
                Debug.Log($"piece mobility {piece.name} piece on board{hiveman.isOnBoard}");
                continue;
            } 
            Debug.Log($"piece mobility {piece.name} piece on board{hiveman.isOnBoard}");
            List<Vector2Int> possibleMoves = GetPossibleMoves(hiveman);
            if (possibleMoves.Count == 0)
            {
                mobilityScore+=EvaluatePiecePriority($"{hiveman.GetName().Substring(2)}");
            }
            Debug.Log($"{EvaluatePiecePriority($"{hiveman.GetName().Substring(2)}")} {hiveman.GetName().Substring(2)}");
            Debug.Log($"{mobilityScore} mobility score in evaluation mobility function");
        }

        return mobilityScore;
    }

    private int Minimax(int depth, bool isMaximizing, int alpha, int beta)
    {
        controller = GameObject.FindWithTag("GameController");
        gamesc = controller.GetComponent<Game>();
        Debug.Log($"Minimax({depth}, {isMaximizing}, {alpha}, {beta})");
        // Base case: terminal condition
        if (depth == 0 || gamesc.IsGameOver()) {
            //return Random.Range(0, 10);
            int score = EvaluateGameState();
            Debug.Log($"score minimaz {score}");
            return score;
        }

        List <GameObject> pieces = isMaximizing ? GetPlayerPieces() : gamesc.GetOpponentPieces("w");
        int bestValue = isMaximizing ? int.MinValue : int.MaxValue;

        bool ant_checked = false;
        bool grasshopper_checked = false;
        bool spider_checked = false;
        bool beetle_checked = false;

        foreach (GameObject piece in pieces)
        {
            Hiveman hiveman = piece.GetComponent<Hiveman>();
            if (hiveman == null) continue;

            List<Vector2Int> possibleMoves = GetPossibleMoves(hiveman);
            if (hiveman.isOnBoard)
            {
                foreach (Vector2Int move in possibleMoves)
                {
                    Dictionary<(int x, int y), Stack<GameObject>> temp_positions = DeepCopyPositions(gamesc.positions); 
                    Vector2Int original = SimulatePlacement(piece, move); // Simulate the move
                    int eval = Minimax(depth - 1, !isMaximizing, alpha, beta);
                    RestoreMove(piece, move, original);
                    gamesc.positions = temp_positions;
                    if (isMaximizing)
                    {
                        bestValue = Mathf.Max(bestValue, eval);
                        alpha = Mathf.Max(alpha, eval);
                    }
                    else
                    {
                        bestValue = Mathf.Min(bestValue, eval);
                        beta = Mathf.Min(beta, eval);
                    }

                    if (beta <= alpha) break;
                }
            }
            else
            {
                if (piece.name == "b_ant") {
                    if (ant_checked) continue;
                    else ant_checked = true;
                }
                if (piece.name == "b_beetle")
                {
                    if (beetle_checked) continue;
                    else beetle_checked = true;
                }
                if (piece.name == "b_spider")
                {
                    if (spider_checked) continue;
                    else spider_checked = true;
                }
                if (piece.name == "b_grasshopper")
                {
                    if (grasshopper_checked) continue;
                    else grasshopper_checked = true;
                }


                foreach (Vector2Int move in possibleMoves)
                {
                    Dictionary<(int x, int y), Stack<GameObject>> temp_positions = DeepCopyPositions(gamesc.positions);
                    Vector2Int original = SimulatePlacement2(piece, move); // Simulate the move
                    int eval = Minimax(depth - 1, !isMaximizing, alpha, beta);
                    RestoreMove2(piece, move, original);
                    gamesc.positions = temp_positions;

                    if (isMaximizing)
                    {
                        bestValue = Mathf.Max(bestValue, eval);
                        alpha = Mathf.Max(alpha, eval);
                    }
                    else
                    {
                        bestValue = Mathf.Min(bestValue, eval);
                        beta = Mathf.Min(beta, eval);
                    }

                    if (beta <= alpha) break;
                }
            }

            if (beta <= alpha) break; // Additional pruning outside inner loop
        }

        return bestValue;
    }

    private int Alpha_Beta(int depth, bool isMaximizing, int alpha, int beta, string currentPlayer)
    {
        if (depth == 0 || gamesc.IsGameOver())
        {
            return EvaluateGameState();
        }

        if (isMaximizing)
        {
            int maxEval = int.MinValue;
            foreach (GameObject piece in gamesc.GetPlayerPieces(currentPlayer))
            {
                Hiveman hiveman = piece.GetComponent<Hiveman>();
                if (hiveman == null) continue;

                List<Vector2Int> possibleMoves = GetPossibleMoves(hiveman);

                foreach (Vector2Int move in possibleMoves)
                {
                    GameState stateBeforeMove = new GameState(gamesc, currentPlayer);
                    SimulateMove(piece, move);

                    int eval = Alpha_Beta(depth - 1, false, alpha, beta, gamesc.GetOpponent(currentPlayer));

                    RestoreGameState(stateBeforeMove,piece);

                    maxEval = Mathf.Max(maxEval, eval);
                    alpha = Mathf.Max(alpha, eval);

                    if (beta <= alpha) break; // Prune the branch
                }
            }
            return maxEval;
        }
        else
        {
            int minEval = int.MaxValue;
            foreach (GameObject piece in gamesc.GetOpponentPieces(currentPlayer))
            {
                Hiveman hiveman = piece.GetComponent<Hiveman>();
                if (hiveman == null) continue;

                List<Vector2Int> possibleMoves = GetPossibleMoves(hiveman);

                foreach (Vector2Int move in possibleMoves)
                {
                    GameState stateBeforeMove = new GameState(gamesc, currentPlayer);
                    SimulateMove(piece, move);

                    int eval = Alpha_Beta(depth - 1, true, alpha, beta, gamesc.GetOpponent(currentPlayer));

                    RestoreGameState(stateBeforeMove, piece);

                    minEval = Mathf.Min(minEval, eval);
                    beta = Mathf.Min(beta, eval);

                    if (beta <= alpha) break; // Prune the branch
                }
            }

            return minEval;
        }
    }

    public void MakeMoveWithIterativeDeepening(string aiPlayer, int timeLimitMs)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        int bestScore = int.MinValue;
        Vector2Int bestMove = Vector2Int.zero;
        GameObject bestPiece = null;

        for (int depth = 1; stopwatch.ElapsedMilliseconds < timeLimitMs; depth++)
        {
            foreach (GameObject piece in gamesc.GetPlayerPieces(aiPlayer))
            {
                Hiveman hiveman = piece.GetComponent<Hiveman>();
                if (hiveman == null) continue;

                List<Vector2Int> possibleMoves = GetPossibleMoves(hiveman);

                foreach (Vector2Int move in possibleMoves)
                {
                    GameState stateBeforeMove = new GameState(gamesc, aiPlayer);
                    SimulateMove(piece, move);

                    int moveScore = Alpha_Beta(depth, false, int.MinValue, int.MaxValue, aiPlayer);

                    RestoreGameState(stateBeforeMove, piece);

                    if (moveScore > bestScore)
                    {
                        bestScore = moveScore;
                        bestMove = move;
                        bestPiece = piece;
                    }
                }
            }
        }

        if (bestMove != Vector2Int.zero && bestPiece != null)
        {
            SimulateMove(bestPiece, bestMove);
            Debug.Log($"{aiPlayer} moved {bestPiece.name} to ({bestMove.x}, {bestMove.y}).");
        }

        stopwatch.Stop();
    }

}
