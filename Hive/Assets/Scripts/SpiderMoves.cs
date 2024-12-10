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
        /*
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
        */
        if (sc.IsBeetleBlocked(x, y, z, currentPlayer))
        {
            return possibleMoves; // Spider cannot move if blocked by a beetle.
        }

        // Start recursive search for paths of exactly 3 steps.
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        List<Vector2Int> currentPath = new List<Vector2Int>();
        Vector2Int start = new Vector2Int(x, y);

        RecursiveSearch(start, 3, visited, currentPath, sc, possibleMoves, currentPlayer);

        return possibleMoves;
    }

    private void RecursiveSearch(
     Vector2Int currentPosition,
     int stepsRemaining,
     HashSet<Vector2Int> visited,
     List<Vector2Int> currentPath,
     Game sc,
     List<Vector2Int> possibleMoves,
     string currentPlayer)
    {
        // If we have made exactly 3 steps, add the final position to possible moves
        if (stepsRemaining == 0)
        {
            possibleMoves.Add(currentPath[currentPath.Count - 1]);
            return;
        }

        visited.Add(currentPosition);
        currentPath.Add(currentPosition);

        foreach (var neighbor in GetNeighbors(currentPosition, sc))
        {
            // Only proceed if the neighbor hasn't been visited yet
            if (!visited.Contains(neighbor))
            {
                RecursiveSearch(neighbor, stepsRemaining - 1, visited, currentPath, sc, possibleMoves, currentPlayer);
            }
        }

        // Backtrack
        visited.Remove(currentPosition);
        currentPath.RemoveAt(currentPath.Count - 1);
    }

    private List<Vector2Int> GetNeighbors(Vector2Int position, Game sc)
    {
        int x = position.x;
        int y = position.y;

        var neighbors = new List<Vector2Int>();

        if (x % 2 == 0)
        {
            neighbors.Add(new Vector2Int(x + 1, y));     // Hex to the right
            neighbors.Add(new Vector2Int(x - 1, y));     // Hex to the left
            neighbors.Add(new Vector2Int(x, y + 1));     // Hex above
            neighbors.Add(new Vector2Int(x, y - 1));     // Hex below
            neighbors.Add(new Vector2Int(x + 1, y + 1)); // Top-right diagonal hex
            neighbors.Add(new Vector2Int(x - 1, y + 1)); // Top-left diagonal hex
        }
        else
        {
            neighbors.Add(new Vector2Int(x + 1, y));     // Hex to the right
            neighbors.Add(new Vector2Int(x - 1, y));     // Hex to the left
            neighbors.Add(new Vector2Int(x, y + 1));     // Hex above
            neighbors.Add(new Vector2Int(x, y - 1));     // Hex below
            neighbors.Add(new Vector2Int(x + 1, y - 1)); // Bottom-right diagonal hex
            neighbors.Add(new Vector2Int(x - 1, y - 1)); // Bottom-left diagonal hex
        }

 

        return neighbors;
    }
}