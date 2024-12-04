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
        List<Vector2Int> GetPossibleMoves(int x, int y, bool isFirstMove, string currentPlayer);
    }
    public IMoveLogic moveLogic;

    //References
    public GameObject controller;
    public GameObject movePlate;

    //Positions
    private int xBoard = -1;    //not on the board yet
    private int yBoard = -1;

    //"black" or "white" player
    private string player;

    //is it the first move for the piece
    private bool isFirstMove = true;
    //will move break the hive
    public bool hiveBreak = false;

    // References for all the sprites the chesspiece can be
    public Sprite b_queenBee, b_ant, b_beetle, b_grasshopper, b_spider;
    public Sprite w_queenBee, w_ant, w_beetle, w_grasshopper, w_spider;

    public bool isOnBoard = false;
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
                //moveLogic = gameObject.AddComponent<AntMoves>();
                break;
            case "b_beetle":
                this.GetComponent<SpriteRenderer>().sprite = b_beetle;
                player = "b";
                moveLogic = gameObject.AddComponent<BeetleMoves>();
                break;
            case "b_grasshopper":
                this.GetComponent<SpriteRenderer>().sprite = b_grasshopper;
                player = "b";
                //moveLogic = gameObject.AddComponent<GrassMoves>();
                break;
            case "b_spider": 
                this.GetComponent<SpriteRenderer>().sprite = b_spider;
                player = "b";
                //moveLogic = gameObject.AddComponent<SpiderMoves>();
                break;

            case "w_queenBee":
                this.GetComponent<SpriteRenderer>().sprite = w_queenBee;
                player = "w";
                moveLogic = gameObject.AddComponent<QueenBeeMoves>();
                break;
            case "w_ant": 
                this.GetComponent<SpriteRenderer>().sprite = w_ant;
                player = "w";
                //moveLogic = gameObject.AddComponent<AntMoves>();
                break;
            case "w_beetle": 
                this.GetComponent<SpriteRenderer>().sprite = w_beetle;
                player = "w";
                moveLogic = gameObject.AddComponent<BeetleMoves>();
                break;
            case "w_grasshopper":
                this.GetComponent<SpriteRenderer>().sprite = w_grasshopper;
                player = "w";
                //moveLogic = gameObject.AddComponent<GrassMoves>();
                break;
            case "w_spider":
                this.GetComponent<SpriteRenderer>().sprite = w_spider;
                player = "w";
                //moveLogic = gameObject.AddComponent<SpiderMoves>();
                break;
        }

    }

    public int GetXBoard() { return this.xBoard; }
    public int GetYBoard() { return this.yBoard; }
    public void SetXBoard(int xBoard) { this.xBoard = xBoard; }
    public void SetYBoard(int yBoard) { this.yBoard = yBoard; }



    //to be edited...
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
    private void OnMouseUp()
    {
        Game sc = controller.GetComponent<Game>();

        // Prevent the piece from being removed or moved off the board
        if (!sc.IsGameOver() && sc.GetCurrentPlayer() == this.player)
        {
            DestroyMovePlates();

            // Check if the piece is already on the board
            if (!isOnBoard)
            {
                // Allow placing the piece for the first time
                isOnBoard = true;
            }
            else
            {
                // Generate move plates only if the piece is already placed
                if (moveLogic != null)
                {
                    List<Vector2Int> possibleMoves = moveLogic.GetPossibleMoves(xBoard, yBoard, isFirstMove, player);
                    foreach (Vector2Int move in possibleMoves)
                    {
                        PointMovePlate(move.x, move.y);
                    }
                }
            }
        }
    }

    public void SetFirstMove(bool firstMove)
    {
        isFirstMove=firstMove;
    }

    public void DestroyMovePlates() {
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("movePlate");
        for (int i = 0; i < movePlates.Length; i++) {
            Destroy(movePlates[i]);
        }
    }

    //function to spawn a moveplate at a certain position
    public void PointMovePlate(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();
        if (sc.IsOnBoard(x, y))
        {

            GameObject cp = sc.GetPosition(x, y);
            if (this.name == "b_beetle" || this.name == "w_beetle")
            {
                if (cp == null)
                {
                    MovePlateSpawn(x, y, false);
                }
                else if (cp.GetComponent<Hiveman>().player != player)
                {

                    MovePlateSpawn(x, y, true);
                }

            }
            else
            {
                if (cp == null)
                {
                    MovePlateSpawn(x, y, false);
                }

            }

        }

    }

    //helper function for PointMovePlate
    public void MovePlateSpawn(int matrixX, int matrixY, bool isOverlap)
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
        if (isOverlap)
        {
            mpScript.overlap = isOverlap;
        }
        mpScript.SetReference(gameObject);

        mpScript.SetCoords(matrixX, matrixY);

    }

}

    

