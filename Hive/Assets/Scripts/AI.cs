// using System;
// using System.Collections.Generic;
// using UnityEngine;
//
// namespace DefaultNamespace
// {
//     public class AI
//     {
//         public GameObject controller;
//         
//
//         public int EvaluateTotalHeuristics(int currentPlayerID)
//         {
//             
//
//             int queenSafetyScore = EvaluateQueenSafety(currentPlayerID);
//             int mobilityScore = EvaluatePieceMobility(currentPlayerID);
//             //    score += EvaluatePieceMobility(board.Player1Pieces) - EvaluatePieceMobility(board.Player2Pieces);
//
//             return mobilityScore + queenSafetyScore;
//         }
//
//         private int EvaluatePieceMobility(int currentPlayerID)
//         {
//             controller = GameObject.FindGameObjectWithTag("GameController");
//             Game sc = controller.GetComponent<Game>();
//             int mobilityScore = 0;
//             
//             // HashSet<Vector2Int> validTiles = sc.GetAdjacentTilesForCurrentPlayer();
//             // foreach (Vector2Int tile in validTiles)
//             // {
//             //     
//             // }
//             
//             foreach (var piece in sc.GetAdjacentTilesForCurrentPlayer())
//             {
//                 mobilityScore += piece.moveLogic.GetPossibleMoves(piece.GetXBoard(), piece.GetYBoard(), piece.GetZBoard(), player).Count;
//             }
//
//             foreach (Piece piece in board.GetPiecesForPlayer(PlayerColor.Player2))
//             {
//                 mobilityScore -= piece.GetPossibleMoves(board).Count;
//             }
//
//             score += mobilityScore;
//
//             return score;
//         }
//
//         private int EvaluateQueenSafety(int currentPlayerID)
//         {
//             throw new System.NotImplementedException();
//         }
//
//         private void MovePiece(GameObject piece, int x, int y)
//         {
//             Hiveman hiveman = piece.GetComponent<Hiveman>();
//             SetPositionEmpty(hiveman.GetXBoard(), hiveman.GetYBoard()); // Clear the current position
//             SetPosition(piece, x, y); // Move to the new position
//             hiveman.SetXBoard(x);
//             hiveman.SetYBoard(y);
//             Debug.Log($"{piece.name} moved to ({x}, {y}).");
//         }
//
//         private void PlacePiece(string pieceName, int x, int y)
//         {
//             GameObject piece = FindPieceByName(pieceName);
//             if (piece != null)
//             {
//                 SetPosition(piece, x, y);
//                 piece.GetComponent<Hiveman>().isOnBoard = true;
//                 Debug.Log($"{pieceName} placed at ({x}, {y}).");
//             }
//             else
//             {
//                 Debug.LogError("Piece not found.");
//             }
//         }
//
//         public (int, move) MinMax(int depth, int currentPlayerID, bool isMaximizingPlayer)
//         {
//             // Base case : stop searching if we reached max depth or the game is over(Queen is surrounded)
//             if (depth == 0 || IsGameOver())
//             {
//                 // Use combined heuristic
//                 return EvaluateTotalHeuristics(currentPlayerIDnull);
//             }
//
//             Hiveman bestmove = null;
//             int bestValue;
//
//             if (isMaximizingPlayer)
//             {
//                 //initialize best value to smallest possible value and then it can take greater values
//                 bestValue = int.MinValue;
//
//                 //loop over all the possible moves the current player can take
//                 foreach (var move in GenerateAllMoves(currentPlayerID))
//                 {
//                     //simulate the game by making a move
//                     MakeMove(move);
//
//                     //Recursively call MinMax for the opponent(Min) at next depth
//                     int value = MinMax(depth - 1, GetOpponentID(currentPlayerID), false);
//
//                     //undo the move to return to original state
//                     UndoMove(move);
//                     if (value > bestValue)
//                     {
//                         bestvalue = value; // Choose the highest score from all possible moves
//                         bestMove = move; // saving the best move
//                     }
//                 }
//             }
//             else //in minimizing player turn
//             {
//                 //initialize best value to the max possible number and then it will decrease
//                 bestValue = int.MaxValue;
//
//                 foreach (var move in GenerateAllMoves(GetOpponentID(currentPlayerID)))
//                 {
//                     MakeMove(move);
//                     int value = MinMax(depth - 1, currentPlayerID, true);
//                     UndoMove(move);
//
//                     if (value < bestValue)
//                     {
//                         bestvalue = value; // saving best value
//                         bestMove = move; // saving the best move
//                     }
//                 }
//             }
//
//             return (bestValue, bestMove);
//         }
//
//         private int GetOpponentID(int currentPlayerID)
//         {
//             return currentPlayerID == 1 ? 2 : 1;
//         }
//
//         public int AlphaBetaPruning(Board board, int depth, int alpha, int beta, bool isMaximizingPlayer)
//         {
//             if (depth == 0 || board.IsTerminalState())
//             {
//                 return EvaluateTotalHeuristics(board);
//             }
//
//             if (isMaximizingPlayer)
//             {
//                 int bestValue = int.MinValue;
//                 foreach (Move move in board.GetPossibleMoves())
//                 {
//                     board.MakeMove(move);
//                     int value = AlphaBetaPruning(board, depth - 1, alpha, beta, false);
//                     board.UndoMove(move);
//                     bestValue = Math.Max(bestValue, value);
//                     alpha = Math.Max(alpha, bestValue);
//                     if (beta <= alpha)
//                     {
//                         break;
//                     }
//                 }
//
//                 return bestValue;
//             }
//             else
//             {
//                 int bestValue = int.MaxValue;
//                 foreach (Move move in board.GetPossibleMoves())
//                 {
//                     board.MakeMove(move);
//                     int value = AlphaBetaPruning(board, depth - 1, alpha, beta, true);
//                     board.UndoMove(move);
//                     bestValue = Math.Min(bestValue, value);
//                     beta = Math.Min(beta, bestValue);
//                     if (beta <= alpha)
//                     {
//                         break;
//                     }
//                 }
//
//                 return bestValue;
//             }
//         }
//     }
//
//     public class Move
//     {
//         public BoardPosition Start { get; set; }
//         public BoardPosition End { get; set; }
//     }
// }