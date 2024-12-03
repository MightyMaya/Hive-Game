
/*
Ants are fast moving and may move any number of hexes across the Hive as long
as they can slide all the way and do not break the One Hive Rule.
*/

using System.Collections.Generic;
using UnityEngine;

public class AntMovement : MonoBehaviour
{
    public Vector2Int Position; // Current position of the ant on the hex grid
    private HiveBoard board; // Reference to the game board


    public List<Vector2Int> GetValidMoves()
    {
        List<Vector2Int> validMoves = new List<Vector2Int>();

        // Perform a breadth-first search (BFS) to find all reachable hexes
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        queue.Enqueue(Position);
        visited.Add(Position);

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            foreach (Vector2Int neighbor in board.GetAdjacentHexes(current))
            {
                if (visited.Contains(neighbor)) continue;

                if (!board.IsHexOccupied(neighbor) && CanSlideInto(current, neighbor) && DoesNotBreakOneHiveRule(Position, neighbor))
                {
                    validMoves.Add(neighbor);
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                }
            }
        }

        return validMoves;
    }

    public void Move(Vector2Int newPosition)
    {
        if (GetValidMoves().Contains(newPosition))
        {
            Position = newPosition;
            board.UpdateAntPosition(this, newPosition);
        }
    }

    private bool CanSlideInto(Vector2Int from, Vector2Int to)
    {
        // Check if the movement from 'from' to 'to' allows sliding
        int occupiedNeighbors = 0;
        List<Vector2Int> neighbors = board.GetAdjacentHexes(to);

        foreach (Vector2Int neighbor in neighbors)
        {
            if (board.IsHexOccupied(neighbor)) occupiedNeighbors++;
        }

        // Sliding requires at least one occupied and one unoccupied adjacent hex
        return occupiedNeighbors >= 1 && !board.IsHexOccupied(from);
    }

    private bool DoesNotBreakOneHiveRule(Vector2Int current, Vector2Int target)
    {
        // Temporarily "move" the ant to test the One Hive Rule
        board.RemovePiece(current);
        bool isOneHive = board.IsOneHive();
        board.PlacePiece(current);

        return isOneHive;
    }
}


/***********************************************************************************************************************
hive board in hiveman.cs will have the following functions 


************************************************************************************************************************/

// Remove a piece from the board

private Dictionary<Vector2Int, Hiveman> boardPositions = new Dictionary<Vector2Int, Hiveman>(); // Maps positions to pieces
    public void RemovePiece(Vector2Int position)
    {
        if (boardPositions.ContainsKey(position))
        {
            boardPositions.Remove(position); // Remove the piece from the board
        }
    }

    // Check if a hex is occupied
    public bool IsHexOccupied(Vector2Int position)
    {
        return boardPositions.ContainsKey(position);
    }

    // Check if the One Hive rule is satisfied (all pieces connected)
    public bool IsOneHive()
    {
        // Essentially, it checks if all the pieces are connected in a single group (no isolated pieces).
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        // Find any piece on the board to start the check
        Vector2Int startPiece = FindAnyPiece();
        if (startPiece == null) return true; // If no pieces exist, the One Hive rule holds

        // Perform BFS or DFS to check connectivity
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(startPiece);
        visited.Add(startPiece);

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            foreach (Vector2Int neighbor in GetNeighbors(current))
            {
                if (!visited.Contains(neighbor) && IsHexOccupied(neighbor))
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }

        // Ensure that all occupied positions are visited (connected)
        foreach (var position in boardPositions.Keys)
        {
            if (!visited.Contains(position)) return false;
        }

        return true;
    }
