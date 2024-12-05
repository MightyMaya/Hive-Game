using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Hiveman;

/*
Beetle Rules:
it moves one space per turn
it can also move on top of the hive
a piece with a beetle on top of it is unable to move
for the purposes of the placing rules, the stack takes on the color of the beetle
from its position on top of the hive, the beetle can move from tile to tile across the top of the hive
it can also drop into spaces that are surrounded and therefore not accessible to most other creature
the only way to block a beetle that on top of the hive is to move another beetle on top of it
all four beetles can be stacked on top of each other
note: when it is first placed, the beetle is placed as the same way as the all other pieces.it cannot be placed directly on top of the hive even though it can be moved there later
*/


public class BeetleMoves : MonoBehaviour, IMoveLogic
{
    public GameObject controller;

   
    /// <summary>
    /// Get possible moves for the beetle at the specified position.
    /// </summary>
    public List<Vector2Int> GetPossibleMoves(int x, int y, string currentPlayer)
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
        else */if (!sc.IsBeetleBlocked(x, y,currentPlayer) ) //if the piece is not blocked by a beetle
        {
            

            // Get all adjacent hexes
            List<Vector2Int> adjacentHexes = GetAdjacentHexes(currentPosition);

            foreach (var hex in adjacentHexes)
            {
                if (sc.IsOnBoard(hex.x, hex.y))
                {
                    GameObject targetTile = sc.GetPosition(x, y);

                    if (targetTile == null)
                    {
                        // Empty space - Beetle can move here
                        possibleMoves.Add(hex);
                    }
                    else
                    {
                        // Check if the tile contains a piec
                        Hiveman hiveman = targetTile.GetComponent<Hiveman>();
                        if (hiveman != null)
                        {
                            // Beetle can stack on another piece
                            possibleMoves.Add(hex);
                        }
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
    }

