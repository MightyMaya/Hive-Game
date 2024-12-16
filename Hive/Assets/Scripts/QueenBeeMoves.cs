using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using static Hiveman;

public class QueenBeeMoves : MonoBehaviour, IMoveLogic
{
    public GameObject controller;

    public List<Vector2Int> GetPossibleMoves(int x, int y, int z, string currentPlayer)
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        Game sc = controller.GetComponent<Game>();

        var possibleMoves = new List<Vector2Int>();

        Vector2Int currentPosition = new Vector2Int(x, y);

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
        if (!sc.IsBeetleBlocked(x, y, z, currentPlayer)) //if the piece is not blocked by a beetle
        {
            // Get all adjacent hexes
            List<Vector2Int> adjacentHexes = GetAdjacentHexes(currentPosition);

            foreach (var hex in adjacentHexes)
            {
                if (sc.IsOnBoard(hex.x, hex.y) && !sc.DoesPieceDisconnectHive(gameObject, hex.x, hex.y))
                   // && !sc.IsPositionBlocked(currentPosition, hex)) // if move does not break the hive and can slide into position
                {
                    GameObject targetTile = sc.GetPosition(hex.x, hex.y);

                    if (targetTile == null)
                    {
                        // Empty space - queen can move here
                        possibleMoves.Add(hex);
                    }
                }
            }
        }

        return possibleMoves;
    }

    /// <summary>
    /// Get all adjacent hexes for a given position.
    /// </summary>
    private List<Vector2Int> GetAdjacentHexes(Vector2Int position)
    {
        var adjacentHexes = new List<Vector2Int>();

        // Add hexes based on board rules (accounting for hexagonal grid)
        if (position.x % 2 == 0)
        {
            adjacentHexes.Add(new Vector2Int(position.x + 1, position.y));
            adjacentHexes.Add(new Vector2Int(position.x - 1, position.y));
            adjacentHexes.Add(new Vector2Int(position.x, position.y + 1));
            adjacentHexes.Add(new Vector2Int(position.x, position.y - 1));
            adjacentHexes.Add(new Vector2Int(position.x + 1, position.y + 1));
            adjacentHexes.Add(new Vector2Int(position.x - 1, position.y + 1));
        }
        else
        {
            adjacentHexes.Add(new Vector2Int(position.x + 1, position.y));
            adjacentHexes.Add(new Vector2Int(position.x - 1, position.y));
            adjacentHexes.Add(new Vector2Int(position.x, position.y + 1));
            adjacentHexes.Add(new Vector2Int(position.x, position.y - 1));
            adjacentHexes.Add(new Vector2Int(position.x + 1, position.y - 1));
            adjacentHexes.Add(new Vector2Int(position.x - 1, position.y - 1));
        }

        return adjacentHexes;
    }

    public bool IsQueenSurrounded(int x, int y, int z, string currentPlayer)
    {
        bool surrounded = true;
        // Get all adjacent hexes
        Vector2Int currentPosition = new Vector2Int(x, y);
        List<Vector2Int> adjacentHexes = GetAdjacentHexes(currentPosition);

        controller = GameObject.FindGameObjectWithTag("GameController");
        Game sc = controller.GetComponent<Game>();

        foreach (var hex in adjacentHexes)
        {
            if (sc.GetPosition(hex.x, hex.y) == null)
            {
                surrounded = false;
            }
        }

        return surrounded;
    }
}
