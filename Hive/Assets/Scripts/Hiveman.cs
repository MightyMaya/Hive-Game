using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class Hiveman : MonoBehaviour
{
    //create an interface for move logic
    public interface IMoveLogic
    {
        //List<Vector2Int> GetPossibleMoves(int x, int y, bool isFirstMove, string currentPlayer);
        List<Vector2Int> GetPossibleMoves(int x, int y, int z, string currentPlayer);
    }
    public IMoveLogic moveLogic;

    //References
    public GameObject controller;
    public GameObject movePlate;

    //Positions
    private int xBoard = -1;    //not on the board yet
    private int yBoard = -1;
    private int zBoard = -1;

    //"black" or "white" player
    public string player;

    //is it the first move for the piece
    //private bool isFirstMove = true;

    //will move break the hive
    public bool hiveBreak = false;

    // References for all the sprites the chesspiece can be
    public Sprite b_queenBee, b_ant, b_beetle, b_grasshopper, b_spider;
    public Sprite w_queenBee, w_ant, w_beetle, w_grasshopper, w_spider;

    public bool isOnBoard = false;
    public bool b_queenBee_isOnBoard = false;
    public bool w_queenBee_isOnBoard = false;

    public void Activate()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        SetCoords();

        switch (this.name)
        {
            case "b_queenBee":
                this.GetComponent<SpriteRenderer>().sprite = b_queenBee;
                player = "b";
                moveLogic = gameObject.AddComponent<QueenBeeMoves>();
                break;
            case "b_ant": 
                this.GetComponent<SpriteRenderer>().sprite = b_ant;
                player = "b";
                moveLogic = gameObject.AddComponent<AntMoves>();
                break;
            case "b_beetle":
                this.GetComponent<SpriteRenderer>().sprite = b_beetle;
                player = "b";
                moveLogic = gameObject.AddComponent<BeetleMoves>();
                break;
            case "b_grasshopper":
                this.GetComponent<SpriteRenderer>().sprite = b_grasshopper;
                player = "b";
                moveLogic = gameObject.AddComponent<GrassMoves>();
                break;
            case "b_spider": 
                this.GetComponent<SpriteRenderer>().sprite = b_spider;
                player = "b";
                moveLogic = gameObject.AddComponent<SpiderMoves>();
                break;

            case "w_queenBee":
                this.GetComponent<SpriteRenderer>().sprite = w_queenBee;
                player = "w";
                moveLogic = gameObject.AddComponent<QueenBeeMoves>();
                break;
            case "w_ant": 
                this.GetComponent<SpriteRenderer>().sprite = w_ant;
                player = "w";
                moveLogic = gameObject.AddComponent<AntMoves>();
                break;
            case "w_beetle": 
                this.GetComponent<SpriteRenderer>().sprite = w_beetle;
                player = "w";
                moveLogic = gameObject.AddComponent<BeetleMoves>();
                break;
            case "w_grasshopper":
                this.GetComponent<SpriteRenderer>().sprite = w_grasshopper;
                player = "w";
                moveLogic = gameObject.AddComponent<GrassMoves>();
                break;
            case "w_spider":
                this.GetComponent<SpriteRenderer>().sprite = w_spider;
                player = "w";
                moveLogic = gameObject.AddComponent<SpiderMoves>();
                break;
        }

    }

    public int GetXBoard() { return this.xBoard; }
    public int GetYBoard() { return this.yBoard; }
    public int GetZBoard() { return this.zBoard; }  
    public void SetXBoard(int xBoard) { this.xBoard = xBoard; }
    public void SetYBoard(int yBoard) { this.yBoard = yBoard; }
    public void SetZBoard(int zBoard) { this.zBoard = zBoard; }



    public void SetCoords()
    {
        float x = xBoard;
        float y = yBoard;
        x *= 0.75f;

        if (xBoard % 2 == 0)
        {
            y *= 1f;
        }
        else {
            y = y - 0.5f;
        }


        x += -9.0f;
        y += -5.0f;
        this.transform.position = new Vector3(x, y, -1.0f);   //-z to be infront of the board not behind it

    }

    //fn to handle user interaction when a piece is clicked
    //generates move plates to indicate possible moves
    private void OnMouseUp()
    {
        Game sc = controller.GetComponent<Game>();

        // Destroy any existing move plates to clear previous highlights
        DestroyMovePlates();

        // Prevent the piece from being removed or moved off the board
        if (sc.IsGameOver() || sc.GetCurrentPlayer() != this.player)
            return;

        // Allow placing the Queen Bee freely (before turn count reaches 4)
        // But restrict other pieces from moving until the Queen Bee is placed
        if (sc.GetCurrentPlayer() == "w" && !sc.IsQueenOnBoard("w") && sc.w_turncount == 3)
        {
            if (this.name != "w_queenBee")
            {
                //Debug.Log($"{player}'s Queen Bee must be placed before any other pieces can move.");
                Debug.Log($"{w_queenBee_isOnBoard}");
                return; // Prevent moving any piece except the Queen Bee
            }
        }
        if (sc.GetCurrentPlayer() == "b" && !sc.IsQueenOnBoard("b") && sc.b_turncount == 3)
        {
            if (this.name != "b_queenBee")
            {
                //Debug.Log($"{player}'s Queen Bee must be placed before any other pieces can move.");
                Debug.Log($"{b_queenBee_isOnBoard}");
                return; // Prevent moving any piece except the Queen Bee
            }
        }


        // Check if the piece is already on the board
        if (!isOnBoard)
        {
            if (sc.isFirstMove) // If it's the first move in the game
            {
                // Highlight the center of the board
                PointMovePlate(14, 5);
            }
            else if (sc.moveCount == 1) //If it is the second move in the game '0 indexed'
            {
                // Second move: Highlight tiles adjacent to all pieces on the board
                HashSet<Vector2Int> validTiles = sc.GetTilesAdjacentToAllPieces();
                foreach (Vector2Int tile in validTiles)
                {
                    PointMovePlate(tile.x, tile.y);
                }
            }
            else 
            {
                // Subsequent moves: Highlight tiles adjacent to pieces of the current player
                HashSet<Vector2Int> validTiles = sc.GetAdjacentTilesForCurrentPlayer();
                foreach (Vector2Int tile in validTiles)
                {
                    PointMovePlate(tile.x, tile.y);
                }
            }
        }
        else  //if Piece is already on the board -> check for the piece allowed moves
        {
            if (moveLogic != null)
            {
                List<Vector2Int> possibleMoves = moveLogic.GetPossibleMoves(xBoard, yBoard, zBoard ,player);
                foreach (Vector2Int move in possibleMoves)
                {
                    PointMovePlate(move.x, move.y);
                }
            }
                
        }
        
    }

    /*public void SetFirstMove(bool firstMove)
    {
        isFirstMove=firstMove;
    }*/

    public void DestroyMovePlates() {
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("movePlate");
        for (int i = 0; i < movePlates.Length; i++) {
            Destroy(movePlates[i]);
        }
    }

    //function to spawn a moveplate at a certain position
    //highlight valid moves for selected piece
    public void PointMovePlate(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();
        if (sc.isFirstMove && x == 14 && y == 5)
        {
            // Highlight the center tile for first move in the game
            MovePlateSpawn(x, y, false, true);
        }
        else if (sc.IsOnBoard(x, y))
        {

            GameObject cp = sc.GetPosition(x, y);
            bool isOverlap = cp != null && cp.GetComponent<Hiveman>().player != player;

            // Highlight the tile only if it's valid for this piece
            if (this.name == "b_beetle" || this.name == "w_beetle")
            {
                MovePlateSpawn(x, y, isOverlap, sc.isFirstMove);
            }
            else if (cp == null) // No overlapping condition for other pieces
            {
                MovePlateSpawn(x, y, false, sc.isFirstMove);
            }

        }
        else
        {
            // Handle the off-board case (optional)
            Debug.Log($"Position ({x}, {y}) is off the board and will not be highlighted.");
        
        }

    }

    //helper function for PointMovePlate
    public void MovePlateSpawn(int matrixX, int matrixY, bool isOverlap, bool isFirstMove)
    {

        float x = matrixX;
        float y = matrixY;

        x *= 0.75f;

        if (matrixX % 2 == 0)
        {
            y *= 1.0f;
        }
        else
        {
            y -= 0.5f;
        }


        x += -9.0f;
        y += -5.0f;

        GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);

        MovePlate mpScript = mp.GetComponent<MovePlate>();

        if (isFirstMove)
        {
            mpScript.isFirstMove = isFirstMove;
        }
        if (isOverlap)
        {
            mpScript.overlap = isOverlap;
        }
        mpScript.SetReference(gameObject);

        mpScript.SetCoords(matrixX, matrixY);

    }

    //public bool IsValidPlacement(int x, int y)
    //{
    //    // Access the Game controller reference from the scene to call IsOnBoard and GetPosition methods
    //    Game gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();

    //    // Check if the position is within the board and the space is empty (no pieces are present)
    //    return gameController.IsOnBoard(x, y) && gameController.GetPosition(x, y) == null;
    //}


    // New: When a piece is moved, record the move in the Game class


    //  TO be moved where our peace Moving Logic exisits
    public void MovePiece(int newX, int newY)
    {
        // Assuming we have a valid move logic here to handle the movement
        Game game = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();

        // Record the move
        game.RecordPlayerMove(player, this.name, xBoard, yBoard); // Record current position as the move

        // Update the piece's new position
        xBoard = newX;
        yBoard = newY;

    }


}



