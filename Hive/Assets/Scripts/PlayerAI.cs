using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using static Hiveman;
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
    public MovePlate aiMovePlate;


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

    // public void StartAI()
    // {
    //     MakeMove(); // Trigger the AI's decision-making process
    // }


    public void MakeMove(string aiPlayer)
    {
        int maxDepth = (int)aiDifficulty; // Use depth based on difficulty
        Vector2Int bestMove = Vector2Int.zero, bestTile = Vector2Int.zero;
        int bestScore = int.MinValue, bestScorePlacement = 0;
        GameObject bestPiece = null, bestPieceToPlace=null;
        Debug.Log("Making move");

        // Loop through all pieces
        foreach (GameObject piece in GetPlayerPieces())
        {
            Hiveman hiveman = piece.GetComponent<Hiveman>();
            List<Vector2Int> possibleMoves = new List<Vector2Int>();

            if (hiveman == null)
            {
                Debug.Log("No hiveman found");
                continue;
                Debug.Log("after continue");
            }

            Debug.Log("After hiveman");
            if (hiveman.isOnBoard)
            {
                // Get all possible moves for the piece
                possibleMoves = GetPossibleMoves(hiveman);

                // List<Vector2Int> possibleMoves = hiveman.moveLogic.GetPossibleMoves(
                //     hiveman.GetXBoard(), hiveman.GetYBoard(), hiveman.GetZBoard(), aiPlayer);
                Debug.Log("Possible moves");
                if (possibleMoves.Count == 0)
                {
                    Debug.Log("No possible moves");
                }

                foreach (Vector2Int move in possibleMoves)
                {
                    Debug.Log($"Possible move for {piece.name}: ({move.x}, {move.y})");
                }

                foreach (Vector2Int move in possibleMoves)
                {
                    Debug.Log($"Evaluating move for {piece.name} to ({move.x}, {move.y})");
                    // Simulate the move
                    GameState stateBeforeMove = SaveGameState();
                    SimulateMove(piece, move);

                    // Run Minimax to get the score
                    int moveScore = Minimax(maxDepth, false, int.MinValue, int.MaxValue);

                    // Undo the move
                    RestoreGameState(stateBeforeMove);

                    Debug.Log($"Move score: {moveScore}");
                    if (moveScore > bestScore)
                    {
                        bestScore = moveScore;
                        bestMove = move;
                        bestPiece = piece;
                    }
                }
            }
            else
            {
                Debug.Log("else placement");
                (bestPieceToPlace, bestTile, bestScorePlacement) = GetBestPlacement(aiPlayer);
                Debug.Log($"piece returned {bestPieceToPlace.name}, score: {bestScorePlacement}");
            }
        }
        
        // Decide whether to place or move based on the best scores
        if (bestScorePlacement >= bestScore && bestPieceToPlace != null)
        {
            // Place the best piece
            MovePiece(bestPieceToPlace, bestTile);
            //aiMovePlate.SetReference(bestPieceToPlace);
            //aiMovePlate.SetCoords(bestTile.x, bestTile.y);
           // aiMovePlate.OnMouseUp();
            Debug.Log($"{aiPlayer} placed {bestPieceToPlace.name} at ({bestTile.x}, {bestTile.y}).");
        }
        else if (bestPiece != null)
        {
            // Move the best piece
            MovePiece(bestPiece, bestMove);
            //aiMovePlate.SetReference(bestPiece);
            //aiMovePlate.SetCoords(bestMove.x, bestMove.y);
            //aiMovePlate.OnMouseUp();
            Debug.Log($"{aiPlayer} moved {bestPiece.name} to ({bestMove.x}, {bestMove.y}).");
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

    public (GameObject bestPiece, Vector2Int bestTile, int bestScore) GetBestPlacement(string player)
    {
        Debug.Log("Getting best piece placement function");
        string[] piecePriority = { "ant", "grasshopper", "spider", "beetle", "queenBee" };
        GameObject bestPieceToPlace = null;
        Vector2Int bestPlacement = Vector2Int.zero;
        int bestScore = int.MinValue;

        foreach (string pieceType in piecePriority)
        {
            GameObject unplacedPiece = gamesc.GetUnplacedPiece(player, pieceType);
            if (unplacedPiece == null) continue;

            HashSet<Vector2Int> validTiles = gamesc.GetTilesAdjacentToAllPieces();

            foreach (Vector2Int tile in validTiles)
            {
                int score = EvaluatePiecePriority(unplacedPiece.name);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestPieceToPlace = unplacedPiece;
                    bestPlacement = tile;
                }
            }
        }

        return (bestPieceToPlace, bestPlacement, bestScore);
    }

    private int EvaluatePiecePriority(string pieceName)
    {
        if (pieceName.Contains("ant")) return 30; // High mobility
        if (pieceName.Contains("grasshopper")) return 20; // Jumping capability
        if (pieceName.Contains("beetle")) return 10; // Defensive
        if (pieceName.Contains("spider")) return 15; // Strategic movement
        if (pieceName.Contains("queenBee")) return 5; // Only if mandatory

        return 0; // Default for unrecognized pieces
    }

    private int EvaluatePlacement(GameObject piece, Vector2Int position, string player)
    {
        // int hiveConnectivity = EvaluateHiveConnectivity(position);
        // int opponentProximity = EvaluateOpponentProximity(position, player);
        // int centralPositioning = EvaluateCentralPositioning(position);
        int piecePriority = EvaluatePiecePriority(piece.name);
        // int queenReadiness = EvaluateQueenReadiness(player);

        // Weighted sum
        return piecePriority;
    }


    private List<Vector2Int> GetPossibleMoves(Hiveman hiveman)
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();
        // Check if the piece is already on the board
        if (!hiveman.isOnBoard)
        {
            if (gamesc.moveCount == 1) //If it is the second move in the game '0 indexed'
            {
                // Second move: Highlight tiles adjacent to all pieces on the board
                possibleMoves = gamesc.GetTilesAdjacentToAllPieces().ToList();
            }
            else
            {
                // Subsequent moves: Highlight tiles adjacent to pieces of the current player
                possibleMoves = gamesc.GetAdjacentTilesForCurrentPlayer().ToList();
            }
        }
        else //if Piece is already on the board and Queen is placed -> check for the piece allowed moves
        {
            if (hiveman.moveLogic != null && gamesc.IsQueenOnBoard(aiPlayer))
            {
                possibleMoves = hiveman.moveLogic.GetPossibleMoves(hiveman.GetXBoard(), hiveman.GetYBoard(),
                    hiveman.GetZBoard(), aiPlayer);
            }
        }

        return possibleMoves;
    }

    private void SimulateMove(GameObject piece, Vector2Int target)
    {
        Hiveman hiveman = piece.GetComponent<Hiveman>();
        if (hiveman != null)
        {
            Debug.Log($"{hiveman} moving to {target}.");
            gamesc.SetPositionEmpty(hiveman.GetXBoard(), hiveman.GetYBoard());
            hiveman.SetXBoard(target.x);
            hiveman.SetYBoard(target.y);
            gamesc.SetPosition(piece);
        }
    }

    public void SimulatePlacement(GameObject piece, Vector2Int position)
    {
        Debug.Log($"{piece} placed to {position}.");
        Hiveman hiveman = piece.GetComponent<Hiveman>();
        hiveman.SetXBoard(position.x);
        hiveman.SetYBoard(position.y);
        hiveman.isOnBoard = true;
        gamesc.SetPosition(piece);
    }
    //------------------------------------------------
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

        // Check for draw condition after the move
        if (gamesc.CheckForDrawDueRedundentMoves())
        {
            gamesc.SetDraw(true);
            gamesc.EndGameDraw();
            Debug.Log("The game is a draw.");
            return; // Stop further game updates
        }

        // Check if the player has valid moves or piece placements
        if (!gamesc.CanPlayerMoveOrPlace(gamesc.GetCurrentPlayer()))
        {
            gamesc.NextTurn(); // Pass the turn to the opponent if no moves are available
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

    private void RestoreGameState(GameState state)
    {
        state.Restore(gamesc, aiPlayer);
    }

    private int EvaluateGameState()
    {
        // Evaluate using heuristics
        int mobilityScore = EvaluatePieceMobility(aiPlayer) - EvaluatePieceMobility(gamesc.GetOpponent("b"));
        // int queenSafetyScore = EvaluateQueenSafety(aiPlayer);

        // Combine heuristics with weights (adjust weights as needed)
        // return 2 * mobilityScore + 5 * queenSafetyScore;
        return mobilityScore;
    }

    private int EvaluateQueenSafety(string s)
    {
        throw new System.NotImplementedException();
    }

    public int EvaluatePieceMobility(string aiPlayer)
    {
        int mobilityScore = 0;
        foreach (GameObject piece in GetPlayerPieces())
        {
            Hiveman hiveman = piece.GetComponent<Hiveman>();
            if (hiveman == null) continue;

            List<Vector2Int> possibleMoves = hiveman.moveLogic.GetPossibleMoves(
                hiveman.GetXBoard(), hiveman.GetYBoard(), hiveman.GetZBoard(), aiPlayer);
            mobilityScore += hiveman.moveLogic
                .GetPossibleMoves(hiveman.GetXBoard(), hiveman.GetYBoard(), hiveman.GetZBoard(), aiPlayer).Count;
        }

        return mobilityScore;
    }

    private int Minimax(int depth, bool isMaximizing, int alpha, int beta)
    {
        if (depth == 0 || gamesc.IsGameOver())
        {
            return EvaluateGameState();
        }

        if (isMaximizing)
        {
            int maxEval = int.MinValue;
            foreach (GameObject piece in GetPlayerPieces())
            {
                Hiveman hiveman = piece.GetComponent<Hiveman>();
                if (hiveman == null) continue;

                List<Vector2Int> possibleMoves = GetPossibleMoves(hiveman);

                foreach (Vector2Int move in possibleMoves)
                {
                    GameState stateBeforeMove = SaveGameState();
                    SimulateMove(piece, move);

                    int eval = Minimax(depth - 1, false, alpha, beta);

                    RestoreGameState(stateBeforeMove);

                    maxEval = Mathf.Max(maxEval, eval);
                    alpha = Mathf.Max(alpha, eval);

                    if (beta <= alpha)
                        break;
                }
            }

            return maxEval;
        }
        else
        {
            int minEval = int.MaxValue;
            foreach (GameObject piece in gamesc.GetOpponentPieces("w"))
            {
                Hiveman hiveman = piece.GetComponent<Hiveman>();
                if (hiveman == null) continue;

                List<Vector2Int> possibleMoves = GetPossibleMoves(hiveman);


                foreach (Vector2Int move in possibleMoves)
                {
                    GameState stateBeforeMove = SaveGameState();
                    SimulateMove(piece, move);

                    int eval = Minimax(depth - 1, true, alpha, beta);

                    RestoreGameState(stateBeforeMove);

                    minEval = Mathf.Min(minEval, eval);
                    beta = Mathf.Min(beta, eval);

                    if (beta <= alpha)
                        break;
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

                    RestoreGameState(stateBeforeMove);

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

                    RestoreGameState(stateBeforeMove);

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

                    RestoreGameState(stateBeforeMove);

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


    /* private void Update()
     {
         if (!actionTriggered && gamesc.GetCurrentPlayer() == aiPlayer)
         {
             //get possible moves
             //b_queen_controller.OnMouseUp();
             //move the piece
            // b_queen_moveplate.OnMouseUp();
             actionTriggered = true; // Set the flag to true
         }
     }

     public void ResetAction()
     {
         // Call this method when you want to reset the flag
         actionTriggered = false;
     }
    */
}