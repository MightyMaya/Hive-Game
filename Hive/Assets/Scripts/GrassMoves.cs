using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Hiveman;

public class GrassMoves : MonoBehaviour, IMoveLogic
{
    public GameObject controller;

    public List<Vector2Int> GetPossibleMoves(int x, int y, int z, string currentPlayer)
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        Game sc = controller.GetComponent<Game>();

        var validMoves = new List<Vector2Int>();
        Vector2Int currentPosition = new Vector2Int(x, y);
        List<Vector2Int> directions = GetValidDirections(currentPosition.x);


        if (!sc.IsBeetleBlocked(x, y, z, currentPlayer)) //if the piece is not blocked by a beetle 
        {

            // Process each direction for a valid jump
            foreach (var direction in directions)
            {

                Vector2Int adjustedDirection = MapDirection(direction, currentPosition.x);  //variable to hold the adjusted direction

                Vector2Int nextPosition = currentPosition + adjustedDirection; // move in the given direction

                //  bool hasJumped = false;

                while (sc.IsOnBoard(nextPosition.x, nextPosition.y))  // Ensure we're within board bounds
                {
                    GameObject targetTile = sc.GetPosition(nextPosition.x, nextPosition.y);

                    //  Debug.Log($"targetT is null  = {targetTile==null} it's X:Y ={nextPosition.x} {nextPosition.y} \n");

                    if (targetTile == null) // No piece here
                    {


                        // if (hasJumped) // If we jumped, this is a valid landing spot
                        //  {

                        //  }
                        break; // Stop further checks in this direction
                    }
                    else
                    {

                        while (targetTile != null)
                        {


                            // If we encounter an occupied tile, mark that we've jumped
                            //  hasJumped = true;

                            // Move to the next position in the same direction
                            adjustedDirection = MapDirection(adjustedDirection, nextPosition.x);
                            nextPosition += adjustedDirection;
                            targetTile = sc.GetPosition(nextPosition.x, nextPosition.y);
                            //adjustedDirection = MapDirection(adjustedDirection, nextPosition.x);


                        }
                        if (!sc.DoesPieceDisconnectHive(gameObject, nextPosition.x, nextPosition.y))
                        {
                            validMoves.Add(nextPosition);
                        }


                    }
                }

            }
        }

        return validMoves;
    }

    /*
    /// <summary>
    /// Dynamically calculate directions based on the current row's parity.
    /// </summary>
    private List<Vector2Int> GetValidDirections(int x)
    {
        List<Vector2Int> directions;
        if (x % 2 == 0)
        {
            directions = new List<Vector2Int>
            {
                new Vector2Int(1, 0),       // Right (down)
                new Vector2Int(-1, 0),      // Left (down)
                new Vector2Int(0, 1),       // Up
                new Vector2Int(0, -1),      // Down
                new Vector2Int(1, 1),       // Diagonal Right-Up
                new Vector2Int(-1, 1),      // Diagonal Left-Up
            };
        }
        else
        {
            directions = new List<Vector2Int>
            {
                new Vector2Int(1, 0),       // Right (up)
                new Vector2Int(-1, 0),      // Left  (up)
                new Vector2Int(0, 1),       // Up
                new Vector2Int(0, -1),      // Down
                new Vector2Int(1, -1),      // Diagonal Right-Down
                new Vector2Int(-1, -1),     // Diagonal Left-Down
            };
        }

        return directions;
    }
    */
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


    /*
//    function to remap the direction based on what row we are on
    private Vector2Int MapDirection(Vector2Int direction, int currentX)
    {
        bool isEvenRow = currentX % 2 == 0;

        // Adjust direction based on row parity
        if (direction == new Vector2Int(1, 0)) // Right
            return isEvenRow ? new Vector2Int(1, 1) : new Vector2Int(1, -1);
        if (direction == new Vector2Int(-1, 0)) // Left
            return isEvenRow ? new Vector2Int(-1, 1): new Vector2Int(-1, -1);
        if (direction == new Vector2Int(1, 1)) // Up-right
            return isEvenRow ? direction : new Vector2Int(1, 0);
        if (direction == new Vector2Int(-1, 1)) // Up-left
            return isEvenRow ? direction : new Vector2Int(-1, 0);
        if (direction == new Vector2Int(1, -1)) // Down-right
            return isEvenRow ? new Vector2Int(1, 0) : direction;
        if (direction == new Vector2Int(-1, -1)) // Down-left
            return isEvenRow ? new Vector2Int(-1, 0) : direction;


        // Default: no remapping
        return direction;
    }
    */

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


}
