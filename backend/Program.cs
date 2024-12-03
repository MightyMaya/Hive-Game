
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
    var possibleMoves = new List<(int, int)>();
    
    // Check all possible directions (like the queen) but with the Spider rule
    var directions = new(int, int)[]
    {
        (1, 0),   // Right
        (-1, 0),  // Left
        (1, 1),   // Top right
        (1, -1),  // Bottom right
        (-1, 1),  // Top left
        (-1, -1)  // Bottom left
    };

    // Try to move in each direction, checking if a Spider can move exactly 3 steps
    foreach (var (dx, dy) in directions)
    {
        var tempX = x;
        var tempY = y;
        int count = 0;
        bool validMove = true;

        // Try moving 3 spaces in one direction
        while (count < 3)
        {
            tempX += dx;
            tempY += dy;

            // If we encounter an occupied position (own piece or any other piece), break out
            if (hiveState.ContainsKey((tempX, tempY)) || !adjacentPiece(tempX, tempY, 0)) 
            {
                validMove = false;
                break;
            }

            count++;
        }

        // If valid and moved 3 steps, add the position
        if (validMove && count == 3)
        {
            possibleMoves.Add((tempX, tempY));
        }
    }

    return possibleMoves;
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
