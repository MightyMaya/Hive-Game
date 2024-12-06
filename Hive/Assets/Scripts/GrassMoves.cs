using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using static Hiveman;

/*
Grasshopper Rules:
Movement Type: The Grasshopper jumps in a straight line over adjacent pieces.
Range: It must jump over at least one piece and land on the next empty space along the line of its jump.
Direction: The jump can be made in any straight direction (orthogonal or diagonal), but it cannot change direction mid-jump.
Restrictions: The Grasshopper cannot move if there are no pieces to jump over in the chosen direction.
*/

//this movement is a placeholder
public class GrassMoves : MonoBehaviour, IMoveLogic
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

