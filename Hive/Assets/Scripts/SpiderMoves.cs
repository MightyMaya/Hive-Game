using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Hiveman;

/*
Spider Rules:
Movement Type: The Spider moves by crawling around the edges of the hive.
Range: It always moves exactly three spaces per turn, no more, no less.
Sliding: Like all pieces in Hive, the Spider must slide between pieces without breaking the hive (ensuring it maintains one contiguous group).
Restrictions: It cannot move onto or over other pieces or climb.
*/

public class SpiderMoves : MonoBehaviour, IMoveLogic
{
    public GameObject controller;

    public List<Vector2Int> GetPossibleMoves(int x, int y, int z, string currentPlayer)
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        Game sc = controller.GetComponent<Game>();

        var validMoves = new List<Vector2Int>();
        Vector2Int currentPosition = new Vector2Int(x, y);

        if (!sc.IsBeetleBlocked(x, y, z, currentPlayer)) // Check if the piece is blocked by a beetle
        {
            // Perform a Depth-First Search (DFS) to validate all 3-move paths
            DFSForSpiderMoves(currentPosition, new List<Vector2Int> { currentPosition }, 3, sc, validMoves);
        }

        return validMoves;
    }
    /*
    private void DFSForSpiderMoves(Vector2Int currentPosition, List<Vector2Int> visited, int depth, Game sc, List<Vector2Int> validMoves)
    {
        if (depth == 0) // Spider must move exactly 3 spaces
        {
            visited.RemoveRange(1, visited.Count - 1);
            if (!sc.DoesPieceDisconnectHive(gameObject, currentPosition.x, currentPosition.y))
            {
                
                validMoves.Add(currentPosition);
            }
            return;
        }

        List<Vector2Int> directions = GetValidDirections(currentPosition.x);

        foreach (var direction in directions)
        {
            Vector2Int nextPosition = currentPosition + MapDirection(direction, currentPosition.x);

            // Check if the next position is valid
            if (sc.IsOnBoard(nextPosition.x, nextPosition.y) &&
                !visited.Contains(nextPosition) && // Prevent backtracking
                sc.GetPosition(nextPosition.x, nextPosition.y) == null)// Ensure the tile is unoccupied
                                                                       //sc.AreTilesAdjacent(currentPosition, nextPosition)) // Ensure sliding around the hive
            {

                visited.Add(nextPosition);
                Debug.Log($"Current Position: {currentPosition}, Depth: {depth}, Visited: {string.Join(", ", visited)}");
                DFSForSpiderMoves(nextPosition, visited, depth - 1, sc, validMoves);
               //visited.Remove(nextPosition); Backtrack to explore other paths
            }

        }
        
    }
    */

    private void DFSForSpiderMoves(Vector2Int currentPosition, List<Vector2Int> visited, int depth, Game sc, List<Vector2Int> validMoves)
    {
        if (depth == 0) // Spider must move exactly 3 spaces
        {
            // Add the current position to validMoves if the visited path is exactly 3 steps
            if (visited.Count == 4 && !sc.DoesPieceDisconnectHive(gameObject, currentPosition.x, currentPosition.y))
            {
                validMoves.Add(currentPosition);
                //remove the explored path from the visited list
                //visited.RemoveRange(1, visited.Count - 1);
            }
            return;
        }

        List<Vector2Int> directions = GetValidDirections(currentPosition.x);

        foreach (var direction in directions)
        {
            Vector2Int nextPosition = currentPosition + MapDirection(direction, currentPosition.x);

            // Check if the next position is valid
            if (sc.IsOnBoard(nextPosition.x, nextPosition.y) &&
                !visited.Contains(nextPosition) && // Prevent backtracking
                sc.GetPosition(nextPosition.x, nextPosition.y) == null && // Ensure the tile is unoccupied
                IsAdjacentToHive(nextPosition, sc)) // Ensure the tile is in contact with the hive
            {
                // Add the next position to the path
                visited.Add(nextPosition);

                // Continue exploring paths with reduced depth
                DFSForSpiderMoves(nextPosition, visited, depth - 1, sc, validMoves);

                // Backtrack to explore other paths
                visited.RemoveAt(visited.Count - 1);
            }
        }
    }


    private List<Vector2Int> GetValidDirections(int x)
    {
        // Include all possible directions regardless of parity
        return new List<Vector2Int>
        {
            new Vector2Int(1, 0),       // Right
            new Vector2Int(-1, 0),      // Left
            new Vector2Int(0, 1),       // Up
            new Vector2Int(0, -1),      // Down
            new Vector2Int(1, 1),       // Diagonal Right-Up
            new Vector2Int(-1, 1),      // Diagonal Left-Up
            new Vector2Int(1, -1),      // Diagonal Right-Down
            new Vector2Int(-1, -1)      // Diagonal Left-Down
        };
    }

    private Vector2Int MapDirection(Vector2Int direction, int currentX)
    {
        bool isEvenRow = currentX % 2 == 0;

        // Vertical directions (Up and Down) remain unaffected by row parity
        if (direction == new Vector2Int(0, 1)) // Up
            return direction;
        if (direction == new Vector2Int(0, -1)) // Down
            return direction;

        // Diagonal and horizontal directions
        if (direction == new Vector2Int(1, 0)) // Right
            return isEvenRow ? new Vector2Int(1, 1) : new Vector2Int(1, -1);
        if (direction == new Vector2Int(-1, 0)) // Left
            return isEvenRow ? new Vector2Int(-1, 1) : new Vector2Int(-1, -1);
        if (direction == new Vector2Int(1, 1)) // Up-right
            return isEvenRow ? new Vector2Int(1, 1) : new Vector2Int(1, 0);
        if (direction == new Vector2Int(-1, 1)) // Up-left
            return isEvenRow ? new Vector2Int(-1, 1) : new Vector2Int(-1, 0);
        if (direction == new Vector2Int(1, -1)) // Down-right
            return isEvenRow ? new Vector2Int(1, 0) : new Vector2Int(1, -1);
        if (direction == new Vector2Int(-1, -1)) // Down-left
            return isEvenRow ? new Vector2Int(-1, 0) : new Vector2Int(-1, -1);

        // Default: no remapping
        return direction;
    }

    //function to check if spider is in contact with another piece
    private bool IsAdjacentToHive(Vector2Int position, Game sc)
    {
        List<Vector2Int> directions = GetValidDirections(position.x);

        foreach (var direction in directions)
        {
            Vector2Int adjacentPosition = position + MapDirection(direction, position.x);

            // Check if the adjacent position is on the board and occupied, but not by the Spider itself
            if (sc.IsOnBoard(adjacentPosition.x, adjacentPosition.y))
            {
                GameObject pieceAtAdjacentPosition = sc.GetPosition(adjacentPosition.x, adjacentPosition.y);
                if (pieceAtAdjacentPosition != null && pieceAtAdjacentPosition != gameObject) // Avoid counting the Spider itself
                {
                    return true; // Found a valid hive connection
                }
            }
        }
        return false;
    }


}
