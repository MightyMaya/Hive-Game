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
public class BeetleMovement 
{
    private Dictionary<Vector2Int, Stack<GameObject>> boardStacks = new Dictionary<Vector2Int, Stack<GameObject>>();
    

    // Method to generate all possible moves for a beetle
    public void GenerateBeetleMoves()
    {
        List<Vector2Int> adjacentHexes = GetAdjacentHexes(new Vector2Int(GetXBoard(), GetYBoard()));

        foreach (Vector2Int hex in adjacentHexes)
        {
            int x = hex.x;
            int y = hex.y;

            if (controller.IsOnBoard(x, y))
            {
                GameObject targetTile = controller.GetPosition(x, y);

                if (targetTile == null)
                {
                    // Empty space - Beetle can move here
                    MovePlateSpawn(x, y, false);
                }
                else
                {
                    // Check if the targetTile contains a beetle
                    Hiveman hiveman = targetTile.GetComponent<Hiveman>();
                    if (hiveman != null && (hiveman.name.StartsWith("b_beetle") || hiveman.name.StartsWith("w_beetle")))
                    {
                        // Beetle can stack on another piece
                        MovePlateSpawn(x, y, true);

                        // Track the stack of beetles at the target position
                        TrackStackedBeetles();
                    }
                }
            }
        }
    }

    // Method to track beetles stacked on top of each other at the current position
    private void TrackStackedBeetles()
    {
        int stackCount = 0;
        int blackBeetles = 0;
        int whiteBeetles = 0;

        Vector2Int position = new Vector2Int(GetXBoard(), GetYBoard());

        // Check if there are any stacked pieces at this position
        if (boardStacks.ContainsKey(position))
        {
            Stack<GameObject> stack = boardStacks[position];

            // Traverse the stack from the top to the bottom
            while (stack.Count > 0)
            {
                GameObject currentTile = stack.Pop(); // Pop the top piece in the stack

                Hiveman hiveman = currentTile.GetComponent<Hiveman>();

                if (hiveman != null)
                {
                    if (hiveman.name.StartsWith("b_beetle"))
                        blackBeetles++;
                    else if (hiveman.name.StartsWith("w_beetle"))
                        whiteBeetles++;
                }

                stackCount++;
            }
        }

        // After the loop, you know the stack count and number of beetles (unity function)
        Debug.Log($"Stack Count: {stackCount}, Black Beetles: {blackBeetles}, White Beetles: {whiteBeetles}");
    }

    // Method to add a piece to the stack at a specific position
    private void AddPieceToStack(Vector2Int position, GameObject piece)
    {
        if (!boardStacks.ContainsKey(position))
        {
            boardStacks[position] = new Stack<GameObject>();
        }

        // Add the piece to the stack at this position
        boardStacks[position].Push(piece);
    }

    // Method to get the piece below the current piece in the stack
    private GameObject GetPieceBelow(Vector2Int position)
    {
        if (boardStacks.ContainsKey(position) && boardStacks[position].Count > 1)
        {
            // Pop the top piece to get the piece below
            boardStacks[position].Pop(); // Remove the top piece
            return boardStacks[position].Peek(); // Return the piece below
        }
        return null; // No piece below 
    }

    // Movement Logic
    public void MoveBeetle(Vector2Int fromPosition, Vector2Int toPosition)
{
    // Get the beetle to move
    GameObject beetleToMove = GetBeetleAtPosition(fromPosition);

    if (beetleToMove != null)
    {
        // Remove beetle from the old stack
        RemoveBeetleFromStack(fromPosition, beetleToMove);

        // Add the beetle to the new stack (at the target position)
        AddPieceToStack(toPosition, beetleToMove);

        // Update the Unity board to reflect the beetle's new position

        // Optionally track the stack after the move
        TrackStackedBeetles();
    }
}

private void RemoveBeetleFromStack(Vector2Int position, GameObject beetle)
{
    if (boardStacks.ContainsKey(position))
    {
        Stack<GameObject> stack = boardStacks[position];

        // If the stack is not empty, pop the beetle
        if (stack.Count > 0 && stack.Peek() == beetle)
        {
            stack.Pop();
            // remove the entry from the dictionary if the stack is empty
            if (stack.Count == 0)
            {
                boardStacks.Remove(position);
            }
        }
    }
}

}