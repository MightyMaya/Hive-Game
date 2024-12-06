using System.Collections;
using System.Collections.Generic;
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
        Vector2Int currentPosition = new Vector2Int(x, y);

        if (!sc.IsBeetleBlocked(x, y,z, currentPlayer)) //if the piece is not blocked by a beetle
        {
            {
                // Perform a flood-fill to find all reachable positions
                var visited = new HashSet<Vector2Int>();
                var stack = new Stack<Vector2Int>();

                stack.Push(currentPosition);

                while (stack.Count > 0)
                {
                    var current = stack.Pop();

                    if (visited.Contains(current))
                        continue;

                    visited.Add(current);

                    // Get all adjacent hexes
                    var adjacentHexes = GetAdjacentHexes(current);

                    foreach (var hex in adjacentHexes)
                    {
                        if (!sc.IsOnBoard(hex.x, hex.y) || visited.Contains(hex))
                            continue;

                        // Check if the hex is empty and the move maintains hive integrity
                        if (sc.GetPosition(hex.x, hex.y) == null)
                        //&& sc.DoesHiveRemainConnected(currentPosition, hex))
                        {
                            possibleMoves.Add(hex);
                            stack.Push(hex);
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
        public List<Vector2Int> GetAdjacentHexes(Vector2Int position)
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



