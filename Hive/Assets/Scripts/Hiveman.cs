using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Hiveman : MonoBehaviour
{
    //References
    public GameObject controller;
    public GameObject movePlate;

    //Positions
    private int xBoard = -1;    //not on the board yet
    private int yBoard = -1;

    //"black" or "white" player
    private string player;

    // References for all the sprites the chesspiece can be
    public Sprite b_queenBee, b_ant, b_beetle, b_grasshopper, b_spider;
    public Sprite w_queenBee, w_ant, w_beetle, w_grasshopper, w_spider;

    public void Activate()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        SetCoords();

        switch (this.name)
        {
            case "b_queenBee": this.GetComponent<SpriteRenderer>().sprite = b_queenBee; player = "b"; break;
            case "b_ant": this.GetComponent<SpriteRenderer>().sprite = b_ant; player = "b"; break;
            case "b_beetle": this.GetComponent<SpriteRenderer>().sprite = b_beetle; player = "b"; break;
            case "b_grasshopper": this.GetComponent<SpriteRenderer>().sprite = b_grasshopper; player = "b"; break;
            case "b_spider": this.GetComponent<SpriteRenderer>().sprite = b_spider; player = "b";  break;

            case "w_queenBee": this.GetComponent<SpriteRenderer>().sprite = w_queenBee; player = "w";  break;
            case "w_ant": this.GetComponent<SpriteRenderer>().sprite = w_ant; player = "w"; break;
            case "w_beetle": this.GetComponent<SpriteRenderer>().sprite = w_beetle; player = "w"; break;
            case "w_grasshopper": this.GetComponent<SpriteRenderer>().sprite = w_grasshopper; player = "w"; break;
            case "w_spider": this.GetComponent<SpriteRenderer>().sprite = w_spider; player = "w"; break;
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
            y *= 0.5f;
        }


        x += -9.0f;
        y += -5.0f;
        this.transform.position = new Vector3(x, y, -1.0f);   //-z to be infront of the board not behind it

    }
    private void OnMouseUp()
    {
        DestroyMovePlates();

        InitiateMovePlates();
    }

    public void DestroyMovePlates() {
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("movePlate");
        for (int i = 0; i < movePlates.Length; i++) {
            Destroy(movePlates[i]);
        }
    }
    //placeholder code
    public void InitiateMovePlates()
    {

        switch (this.name)
        {
            //can't move on the top of the hive
            case "b_queenBee":
            case "w_queenBee":
                LineMovePlate(1, 0);
                //LineMovePlate(-1, 0);
               // LineMovePlate(1, -1);
               // LineMovePlate(-1, -1);
                break;
            //can move on the top of the hive
            case "b_beetle":
            case "w_beetle":
                LineMovePlate(1, 0);
               // LineMovePlate(-1, 0);
               // LineMovePlate(1, -1);
               // LineMovePlate(-1, -1);
                break;



        }
    }

    //example for a move for a certain hivepiece (placeholder)
    public void LineMovePlate(int xIncrement, int yIncrement)
    {
        Game sc = controller.GetComponent<Game>();
        int x = xBoard + xIncrement;
        int y = yBoard + yIncrement;

        while (sc.IsOnBoard(x, y) && sc.GetPosition(x, y) == null)
        {
            MovePlateSpawn(x, y, false);
            x += xIncrement;
            y += yIncrement;
        }

        if (sc.IsOnBoard(x, y) && sc.GetPosition(x, y).GetComponent<Hiveman>().player != player)
        {
            MovePlateSpawn(x, y, true);


        }



    }
    //this will be used inside a move function for a certain piece.
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

    public List<Vector2Int> GetAdjacentHexes(Vector2Int position)
    {
        return new List<Vector2Int>
    {
        new Vector2Int(position.x + 1, position.y),     // Hex to the right
        new Vector2Int(position.x - 1, position.y),     // Hex to the left
        new Vector2Int(position.x, position.y + 1),     // Hex above
        new Vector2Int(position.x, position.y - 1),     // Hex below
        new Vector2Int(position.x + 1, position.y - 1), // Top-right diagonal hex
        new Vector2Int(position.x - 1, position.y + 1)  // Bottom-left diagonal hex
    };
    }


    public void MovePlateSpawn(int matrixX, int matrixY, bool isOverlap)
    {

        float x = matrixX;
        float y = matrixY;

        x *= 0.75f;

        if (matrixX % 2 == 0)
        {
            y *= 1f;
        }
        else
        {
            y *= 0.5f;
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



