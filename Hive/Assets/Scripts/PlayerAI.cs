using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using static Hiveman;
using static UnityEngine.GraphicsBuffer;
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

    // private int maxDepth = 3; // Depth limit for Minimax

    public enum Difficulty
    {
        Easy = 2, // Shallow depth
        Medium = 4, // Moderate depth
        Hard = 6 // Deep search
    }

    public Difficulty aiDifficulty = Difficulty.Medium;


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

    public void MakeMove(string aiPlayer)
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
            if (hiveman == null)
            {
                Debug.Log("No hiveman found on piece.");
                continue;
            }
            // Evaluate possible moves for pieces on the board
            List<Vector2Int> possibleMoves = GetPossibleMoves(hiveman);
            foreach (Vector2Int move in possibleMoves)
            {
                Debug.Log($"Evaluating move for {piece.name} to ({move.x}, {move.y})");
                GameState stateBeforeMove = SaveGameState(); // Save game state before simulating
                SimulatePlacement(piece, move); // Simulate the move
                if (gamesc.DoesPieceDisconnectHive(piece, move.x, move.y))
                {
                    Debug.Log("whyy did you do that");
                    RestoreGameState(stateBeforeMove, piece);
                    continue;
                }
                Debug.Log("in min max");

                int moveScore = Minimax(maxDepth, false, int.MinValue, int.MaxValue);
                RestoreGameState(stateBeforeMove, piece); // Restore game state

                Debug.Log("outa min max");

                Debug.Log($"Move score for {piece.name}: {moveScore}");
                if (moveScore > bestMoveScore)
                {
                    bestMoveScore = moveScore;
                    bestPieceToMove = piece;
                    bestMove = move;
                }
            }
        }

        // Decide whether to move or place based on the best scores
        if (bestPlacementScore >= bestMoveScore && bestPieceToPlace != null)
        {
            // Place the piece with the best placement score
            MovePiece(bestPieceToPlace, bestTileToPlace);
            Debug.Log($"{aiPlayer} placed {bestPieceToPlace.name} at ({bestTileToPlace.x}, {bestTileToPlace.y}).");
        }
        else if (bestPieceToMove != null)
        {
            // Move the piece with the best move score
            MovePiece(bestPieceToMove, bestMove);
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

    public (GameObject bestPiece, Vector2Int bestTile, int bestScore) GetBestPlacement(GameObject piece, string player)
    {
        Debug.Log($"Evaluating best placement for piece: {piece.name}");

        GameObject bestPieceToPlace = null;
        Vector2Int bestPlacement = Vector2Int.zero;
        int bestScore = int.MinValue;
        int maxDepth = (int)aiDifficulty; // Use depth based on difficulty

        // Get valid tiles adjacent to all pieces
        HashSet<Vector2Int> validTiles = gamesc.GetTilesAdjacentToAllPieces();

        if (validTiles == null || validTiles.Count == 0)
        {
            Debug.LogWarning("No valid tiles found for placement.");
            return (null, Vector2Int.zero, int.MinValue);
        }

        Debug.Log($"Valid tiles for placement: {string.Join(", ", validTiles)}");

        foreach (Vector2Int tile in validTiles)
        {
            Debug.Log($"Simulating placement of {piece.name} at tile ({tile.x}, {tile.y})");


            // Save the current game state
            GameState stateBeforePlacement = SaveGameState();

            // Simulate placing the piece
            SimulatePlacement(piece, tile);



            // Run Minimax to evaluate the placement
            int score = Minimax(maxDepth, false, int.MinValue, int.MaxValue);

            // Restore the game state
            RestoreGameState(stateBeforePlacement, piece);

            Debug.Log($"Placement score for {piece.name} at ({tile.x}, {tile.y}): {score}");

            // Update the best placement if the score is higher
            if (score > bestScore)
            {
                bestScore = score;
                bestPieceToPlace = piece;
                bestPlacement = tile;

                Debug.Log($"New best placement found: {piece.name} at ({tile.x}, {tile.y}) with score {score}");
            }
        }

        if (bestPieceToPlace == null)
        {
            Debug.LogWarning("No valid placements resulted in a better score.");
        }

        return (bestPieceToPlace, bestPlacement, bestScore);
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
    
   


    private int EvaluatePlacement(GameObject piece, Vector2Int position, string player)
    {
        // int hiveConnectivity = EvaluateHiveConnectivity(position);
        // int opponentProximity = EvaluateOpponentProximity(position, player);
        // int centralPositioning = EvaluateCentralPositioning(position);
        // int piecePriority = EvaluatePiecePriority(piece.name);
        int pieceMobility = EvaluatePieceMobility(player);
        // int queenSafty = (gamesc.IsQueenOnBoard(player))?EvaluateQueenSafety(piece, piece.GetComponent<Hiveman>().GetPieceType(piece,player)):0;
        // int queenReadiness = EvaluateQueenReadiness(player);

        // Weighted sum
        int score = 5 * pieceMobility;//10*queenSafty+5*pieceMobility;
        return score;
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
    public int EvaluateQueenSafety(GameObject queen, bool isAIPlayer)
    {
        Hiveman hiveman = queen.GetComponent<Hiveman>();
        if (hiveman == null)
        {
            Debug.LogError("Queen is null or missing Hiveman component!");
            return 0;
        }

        // Get the current player's and opponent's piece type
        string playerType = isAIPlayer ? "friendly" : "opponent";
        string opponentType = isAIPlayer ? "opponent" : "friendly";

        // Calculate the number of surrounding opponent and friendly pieces
        int opponentSurrounding = GetSurroundingPiecesCount(queen, opponentType);
        int friendlySupport = GetSurroundingPiecesCount(queen, playerType);

        // Calculate the queen's mobility
        int queenMobility = GetPossibleMoves(hiveman).Count;

        // Apply weights to calculate the heuristic value
        int heuristicValue = (queenMobility * 5) + (friendlySupport * 3)
                             - (opponentSurrounding * 7);

        Debug.Log($"Queen Safety Score (isAIPlayer={isAIPlayer}): {heuristicValue}");
        return heuristicValue;
    }

    private Dictionary<(int x, int y), Stack<GameObject>> positions = new Dictionary<(int, int), Stack<GameObject>>();


// Function to get the number of surrounding pieces of a specific type (friendly or opponent)
public int GetSurroundingPiecesCount(GameObject piece, string pieceType)
{

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

        // Check if there are any pieces on the adjacent tile
        if (positions.ContainsKey(position) && positions[position].Count > 0)
        {
            // Check the piece type on the adjacent tile
            GameObject adjacentPiece = positions[position].Peek();  // Get the top piece
            string currentPieceType = adjacentPiece.GetComponent<Hiveman>().GetPieceType(adjacentPiece,gamesc.GetCurrentPlayer());  // Assuming GetPieceType() returns "friendly" or "opponent"

            // Compare the piece type to the specified one
            if (currentPieceType.Equals(pieceType))
            {
                surroundingCount++;
            }
        }
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
                Debug.Log("Second move of the game: Highlighting tiles adjacent to all pieces.");
                possibleMoves = gamesc.GetTilesAdjacentToAllPieces().ToList();
            }
            else
            {
                Debug.Log("Subsequent move: Highlighting tiles adjacent to the current player's pieces.");
                possibleMoves = gamesc.GetAdjacentTilesForCurrentPlayer().ToList();
            }
        }
        else
        {
            if (hiveman.moveLogic == null)
            {
                Debug.LogError($"{hiveman.name} does not have a moveLogic assigned!");
                return possibleMoves;
            }

            if (gamesc.IsQueenOnBoard(aiPlayer))
            {
                Debug.Log($"Fetching moves for ai {hiveman.name} on the board.");
                possibleMoves = hiveman.moveLogic.GetPossibleMoves(hiveman.GetXBoard(), hiveman.GetYBoard(),
                    hiveman.GetZBoard(), aiPlayer);
            }
            else
            {
                Debug.LogWarning($"Queen is not on the board yet for ai player {aiPlayer}. No moves allowed.");
            }
        }

        Debug.Log($"Possible moves for ai {hiveman.name}: {possibleMoves.Count} moves found.");
        return possibleMoves;
    }


    private void SimulateMove(GameObject piece, Vector2Int target)
    {
        Hiveman hiveman = piece.GetComponent<Hiveman>();
        if (hiveman == null) return;

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
    }

    private void RestoreMove(GameObject piece, int originalX, int originalY)
    {
        Hiveman hiveman = piece.GetComponent<Hiveman>();
        if (hiveman == null) return;

        // Remove the piece from its current position
        gamesc.SetPositionEmpty(hiveman.GetXBoard(), hiveman.GetYBoard());

        // Restore the piece's original position
        hiveman.SetXBoard(originalX);
        hiveman.SetYBoard(originalY);

        // Update the game board with the restored position
        gamesc.SetPosition(piece);

        Debug.Log($"{hiveman.name} restored to ({originalX}, {originalY})");
    }


    public void SimulatePlacement(GameObject piece, Vector2Int position)
    {
        Hiveman hiveman = piece.GetComponent<Hiveman>();
        Debug.Log($"bef simulate{hiveman.isOnBoard}");
        Debug.Log($"{piece} placed to {position}.");
        hiveman.SetXBoard(position.x);
        hiveman.SetYBoard(position.y);
        hiveman.isOnBoard = true;
        gamesc.SetPosition(piece);
        Debug.Log($"after simulate{hiveman.isOnBoard}.");
    }

    public GameObject Controller;

    //reference to the piece that created the moveplate

    public void MovePiece(GameObject piece, Vector2Int targetPosition)
    {
        // Get the game controller object
        Controller = GameObject.FindGameObjectWithTag("GameController");
        Game gamesc = Controller.GetComponent<Game>();

        Hiveman hivesc = piece.GetComponent<Hiveman>();

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

            // Increment the move counter
            gamesc.moveCount++;

            // Mark the piece as being on the board (if it wasn’t already)
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


    private GameState SaveGameState()
    {
        return new GameState(gamesc, aiPlayer); // Implement a class to store game state
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
        int mobilityScore = EvaluatePieceMobility(aiPlayer) - EvaluatePieceMobility(gamesc.GetOpponent("b"));
         //int queenSafetyScore = EvaluateQueenSafety(aiPlayer);
        // Combine heuristics with weights (adjust weights as needed)
        // return 2 * mobilityScore + 5 * queenSafetyScore;
        return mobilityScore;
    }
    

    public int EvaluatePieceMobility(string aiPlayer)
    {
        int mobilityScore = 0;
        foreach (GameObject piece in GetPlayerPieces())
        {
            Hiveman hiveman = piece.GetComponent<Hiveman>();
            if (hiveman == null || !hiveman.isOnBoard) continue;

            List<Vector2Int> possibleMoves = GetPossibleMoves(hiveman);
            if (possibleMoves.Count == 0)
            {
                mobilityScore-=EvaluatePiecePriority($"{this.aiPlayer}_hiveman.GetName()");
            }
            mobilityScore += EvaluatePiecePriority($"{this.aiPlayer}_hiveman.GetName()")*possibleMoves.Count;
        }

        return mobilityScore;
    }

    private int Minimax(int depth, bool isMaximizing, int alpha, int beta)
    {
        // Base case: terminal condition
        if (depth == 0 || gamesc.IsGameOver())
        {
            int randomInt = Random.Range(0, 10); // Generates a random number between 0 (inclusive) and 10 (exclusive)
            int score = randomInt;// EvaluateGameState() + EvaluateQueenSafety(b_queen, isMaximizing);
            Debug.Log($"{score}awful score");

            return score;
        }

        if (isMaximizing)
        {
            Debug.Log("max");
        int maxEval = int.MinValue;

            foreach (GameObject piece in GetPlayerPieces())
            {
                Hiveman hiveman = piece.GetComponent<Hiveman>();
                if (hiveman == null || !hiveman.isOnBoard)
                {
                    Debug.Log($"maxxxxx{hiveman.name}");
                    continue;
                }
                Debug.Log($"max{hiveman.name}");

                // Recursive call
                int eval = Minimax(depth - 1, false, alpha, beta);
                    // Update alpha and maxEval
                    maxEval = Mathf.Max(maxEval, eval);
                    alpha = Mathf.Max(alpha, eval);

                    if (beta <= alpha)
                    {
                        // Alpha-beta pruning
                        return maxEval;
                    }
                
            }

            return maxEval;
        }
        else
        {
            Debug.Log("min");

            int minEval = int.MaxValue;

            foreach (GameObject piece in gamesc.GetOpponentPieces("b"))
            {
                Hiveman hiveman = piece.GetComponent<Hiveman>();
                if (hiveman == null || !hiveman.isOnBoard)
                {
                    Debug.Log($"minnnnnnnnnnnnn{hiveman.name}");
                    continue;
                }
                Debug.Log($"min{hiveman.name}");

                // Recursive call
                int eval = Minimax(depth - 1, true, alpha, beta);
                    // Update beta and minEval
                    minEval = Mathf.Min(minEval, eval);
                    beta = Mathf.Min(beta, eval);

                    if (beta <= alpha)
                    {
                        // Alpha-beta pruning
                        return minEval;
                    }
            }

            return minEval;
        }
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