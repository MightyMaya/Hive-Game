using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using static Hiveman;

/*
Spider Rules:
Movement Type: The Spider moves by crawling around the edges of the hive.
Range: It always moves exactly three spaces per turn, no more, no less.
Sliding: Like all pieces in Hive, the Spider must slide between pieces without breaking the hive (ensuring it maintains one contiguous group).
Restrictions: It cannot move onto or over other pieces or climb
*/

//this movement is a placeholder
public class SpiderMoves : MonoBehaviour, IMoveLogic
{
    public GameObject controller;

    public List<Vector2Int> GetPossibleMoves(int x, int y, int z, string currentPlayer)
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        Game sc = controller.GetComponent<Game>();
        var possibleMoves = new List<Vector2Int>();

        /*if (isFirstMove)
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
        else*/
        if (!sc.IsBeetleBlocked(x, y,z, currentPlayer)) //if the piece is not blocked by a beetle
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

