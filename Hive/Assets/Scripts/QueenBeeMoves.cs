using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using static Hiveman;

public class QueenBeeMoves : MonoBehaviour, IMoveLogic
{
    public List<Vector2Int> GetPossibleMoves(int x, int y, bool isFirstMove)
    {
        var possibleMoves = new List<Vector2Int>();

        if (isFirstMove)
        {
            // Allow movement to any position on the board for the first move.
            int maxX = 29; 
            int maxY = 12; 

            for (int row = 0; row < maxX; row++)
            {
                for (int col = 0; col < maxY; col++)
                {
                    possibleMoves.Add(new Vector2Int(row, col));
                }
            }
        }
        else
        {
            // Standard queen movement logic
            if (x % 2 == 0)
            {
                possibleMoves.Add(new Vector2Int(x + 1, y));     // Hex to the right
                possibleMoves.Add(new Vector2Int(x - 1, y));     // Hex to the left
                possibleMoves.Add(new Vector2Int(x, y + 1));     // Hex above
                possibleMoves.Add(new Vector2Int(x, y - 1));     // Hex below
                possibleMoves.Add(new Vector2Int(x + 1, y + 1)); // Top-right diagonal hex
                possibleMoves.Add(new Vector2Int(x - 1, y + 1)); // Top-left diagonal hex
            }
            else
            {
                possibleMoves.Add(new Vector2Int(x + 1, y));     // Hex to the right
                possibleMoves.Add(new Vector2Int(x - 1, y));     // Hex to the left
                possibleMoves.Add(new Vector2Int(x, y + 1));     // Hex above
                possibleMoves.Add(new Vector2Int(x, y - 1));     // Hex below
                possibleMoves.Add(new Vector2Int(x + 1, y - 1)); // Bottom-right diagonal hex
                possibleMoves.Add(new Vector2Int(x - 1, y - 1)); // Bottom-left diagonal hex
            }
        }

        return possibleMoves;
    }

}

