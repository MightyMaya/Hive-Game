
using System;
using System.Collections.Generic;

public enum PieceType
{
    QUEEN = 1,
    BEETLE,
    ANT,
    GRASSHOPPER,
    SPIDER
}

public class HiveGame
{
    // Dictionary to store the state space representation of the game board.
    // Key: position of piece in HexGrid
    // Value: Stack of Pieces type at that position
    private Dictionary<(int, int), (int PlayerID,Stack<PieceType>)> hiveState = new Dictionary<(int, int), (int,Stack<PieceType>)>();

    //function to make sure there's an adjacent piece to the piece we want to add

    public bool adjacentPiece(int x, int y,int playerID){
        var directions= new(int,int)[]{
            (1,0),
            (-1,0),
            (1,1),
            (1,-1),
            (-1,1),
            (-1,-1)
        };
        foreach(var (dx,dy) in directions){
        var adjX= x+dx;
        var adjY= y+dy;
        if(hiveState.ContainsKey((adjX,adjY)) && hiveState[(adjX,adjY)].PlayerID== playerID){ //Ensure an adjacent piece belonging to the same player exists
            return true;
        }
        }
        return false;
    }
    //function to add new piece to the HexGrid
    public void AddPiece(int x, int y, int PlayerID,PieceType pieceType)
    {
        var position = (x, y);
        if (!hiveState.ContainsKey(position) && adjacentPiece(x,y,PlayerID)) //Ensure that this position is empty and there exists a piece adjacent to it
        {
            hiveState[position] = (PlayerID,new Stack<PieceType>());
        }
        else if(pieceType == PieceType.BEETLE){
            var current= hiveState[position];
            current.Item2.Push(pieceType); //In case position is not empty, a Beetle can be added to the top of stack
            hiveState[position]=current;
        }
    }


    public void RemoveTopPiece(int x, int y)
    {
        var position = (x, y);
        if (hiveState.ContainsKey(position) && hiveState[position].Item2.Count > 0) // Ensure position is not empty
        {
         hiveState[position].Item2.Pop(); //pop the piece off the stack
        }
    }

    //function to get valid moves for each piece
    public List<(int, int)> GetPossibleMoves(int x, int y, PieceType pieceType)
    {
        return pieceType switch
        {
            PieceType.QUEEN => GetQueenMoves(x, y),
            PieceType.SPIDER => GetSpiderMoves(x, y),
            //PieceType.GRASSHOPPER => GetGrasshopperMoves(x, y),
           
           
        };
    }

    public List<(int,int)> GetQueenMoves(int x, int y){
        //Get all Possible moves for the Queen
             var PossibleMoves= new List<(int,int)>
{
    (x + 1, y),     // Move to the right 
    (x - 1, y),     // Move to the left 
    (x + 1, y - 1), // Move diagonally to the bottom right 
    (x - 1, y + 1), // Move diagonally to the top left 
    (x + 1, y + 1), // Move diagonally to the top right 
    (x - 1, y - 1)  // Move diagonally to the bottom left
};

	var validMoves= new List<(int,int)>{};

    //Filter Possible Moves and store only the valid ones(empty positions) in validMoves
	foreach(var position in PossibleMoves){
	if(!hiveState.ContainsKey(position))
		validMoves.Add(position);
    }
    return validMoves;
    }
}

// Add Spider logic to GetPossibleMoves

public List<(int, int)> GetSpiderMoves(int x, int y)
    {
        var validMoves = new List<(int, int)>();
        var directions = new(int, int)[]
        {
            (1, 0),
            (-1, 0),
            (1, 1),
            (1, -1),
            (-1, 1),
            (-1, -1)
        };

        void ExplorePath(int currentX, int currentY, int steps, HashSet<(int, int)> visited)
        {
            if (steps == 3)
            {
                validMoves.Add((currentX, currentY));
                return;
            }

            foreach (var (dx, dy) in directions)
            {
                var nextX = currentX + dx;
                var nextY = currentY + dy;

                if (!visited.Contains((nextX, nextY)) &&
                    !hiveState.ContainsKey((nextX, nextY)) &&
                    adjacentPiece(nextX, nextY, 0))
                {
                    visited.Add((nextX, nextY));
                    ExplorePath(nextX, nextY, steps + 1, visited);
                    visited.Remove((nextX, nextY));
                }
            }
        }

        var visited = new HashSet<(int, int)> { (x, y) };
        ExplorePath(x, y, 0, visited);

        return validMoves;
    }
	public int EvaluateTotalHeuristics(int currentPlayerID)
{
    int queenSafetyScore = EvaluateQueenSafety(currentPlayerID);
    int mobilityScore = EvaluatePieceMobility(currentPlayerID);
		
    return mobilityScore + queenSafetyScore;
}
	public (int,move) MinMax(int depth, int currentPlayerID, bool isMaximizingPlayer)
{
    // Base case : stop searching if we reached max depth or the game is over(Queen is surrounded)
    if (depth == 0 || IsGameOver())
    {
        // Use combined heuristic
        return  EvaluateTotalHeuristics(currentPlayerIDnull);
    }

    Move bestmove=null;
    int bestValue;

    if (isMaximizingPlayer)
    {
	//initialize best value to smallest possible value and then it can take greater values
        bestValue = int.MinValue;

	//loop over all the possible moves the current player can take
        foreach (var move in GenerateAllMoves(currentPlayerID))
        {
	    //simulate the game by making a move
            MakeMove(move);
		
	    //Recursively call MinMax for the opponent(Min) at next depth
            int value = MinMax(depth - 1, GetOpponentID(currentPlayerID), false);

	    //undo the move to return to original state
            UndoMove(move);
	    if ( value > bestValue){
		bestvalue = value; 	    // Choose the highest score from all possible moves
		bestMove = move;	   // saving the best move
	    }
          
        }
    }
    else //in minimizing player turn
    {
	//initialize best value to the max possible number and then it will decrease
        bestValue = int.MaxValue;

        foreach (var move in GenerateAllMoves(GetOpponentID(currentPlayerID)))
        {
            MakeMove(move);
            int value = MinMax(depth - 1, currentPlayerID, true);
            UndoMove(move);

              if ( value < bestValue){
		bestvalue = value; 	    // saving best value
		bestMove = move;	   // saving the best move
	    }
        }
    }

    return (bestValue,bestMove);
}

private int GetOpponentID(int currentPlayerID)
{
    return currentPlayerID == 1 ? 2 : 1;
}

public List<(int,int)>  GetGrass_ShopperMoves(int x, int y){
        //List of adjacent positions of the piece
        var directions = new List<(int, int)> {
        (1, 0),   // Right
        (-1, 0),  // Left
        (1, -1),  // Diagonal bottom right
        (-1, 1),  // Diagonal top left
        (1, 1),   // Diagonal bottom left
        (-1, -1)  // Diagonal top right
    };

    //List to store valid moves 
    var validMoves= new List<(int,int)>{};

    foreach (var direction in directions){
        int adj_x= x+ direction.Item1;
        int adj_y= y+direction.Item2;
        var adj_pos= (adj_x,adj_y);
        if(hiveState.ContainsKey(adj_pos) && hiveState[adj_pos].Item2.Peek() != null){ //checks if there is an adjacent piece that the grass shopper can jump over
            while(hiveState.ContainsKey(adj_pos) && hiveState[adj_pos].Item2.Peek() != null){ //keeps looping until we find an adjacent empty space after making sure there exists and adjacent piece
                adj_pos=(adj_pos.adj_x+direction.Item1, adj_pos.adj_y+direction.Item2);//keep moving in same direction till you find empty space
            }
            validMoves.Add(adj_pos);//add first empty position to the list of valied moves

        }         
        }

    return validMoves;
    }
}
 class Program
    {
        static void Main(string[] args)
        {
            // Your main program logic here
            //Console.WriteLine("Hello, Hive Game!");
        }

}
