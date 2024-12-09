using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;


using static Hiveman;

/*
Ant Rules:
Movement Type: The Ant crawls along the edges of the hive, similar to the Spider.
Range: It can move an unlimited number of spaces around the hive in a single turn.
Sliding: The Ant must slide between pieces without breaking the hive, ensuring it remains a single contiguous group.
Restrictions: Like other pieces, the Ant cannot climb over or move through other pieces.
*/


public class AntMoves : MonoBehaviour, IMoveLogic
{
    public GameObject controller;

    /// <summary>
    /// Get possible moves for the ant at the specified position.
    /// </summary>
    public List<Vector2Int> GetPossibleMoves(int x, int y, int z, string currentPlayer)
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        Game sc = controller.GetComponent<Game>();

        var possibleMoves = new List<Vector2Int>();
        UnityEngine.Debug.Log($"Current Position: ({x}, {y})");

        if (!sc.IsBeetleBlocked(x, y, z, currentPlayer)) // If the piece is not blocked by a beetle
        {
            // Loop through the entire board (assuming a 2D grid, adjust according to your board size)
            for (int i = 0; i < 30 ; i++)  
            {
                for (int j = 0; j < 12 ; j++)  
                {
                    Vector2Int hex = new Vector2Int(i, j);

                    // Skip if the hex is the current position (no need to move there)
                    if (hex == new Vector2Int(x, y)) continue;

                    // Check if the hex is empty and the move maintains hive integrity
                    if (sc.GetPosition(hex.x, hex.y) == null && !sc.DoesPieceDisconnectHive(gameObject, hex.x, hex.y))
                    {
                        possibleMoves.Add(hex); // Add valid move to the list
                        //UnityEngine.Debug.Log("Possible Moves: " + string.Join(", ", possibleMoves.Select(m => $"({m.x}, {m.y})")));

                    }
                }
            }
        }

        return possibleMoves;
    }
}
