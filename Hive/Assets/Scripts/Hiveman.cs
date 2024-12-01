using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            case "b_queenBee": this.GetComponent<SpriteRenderer>().sprite = b_queenBee ; break;
            case "b_ant": this.GetComponent<SpriteRenderer>().sprite = b_ant; break;
            case "b_beetle": this.GetComponent<SpriteRenderer>().sprite = b_beetle; break;
            case "b_grasshopper": this.GetComponent<SpriteRenderer>().sprite = b_grasshopper; break;
            case "b_spider": this.GetComponent<SpriteRenderer>().sprite = b_spider; break;

            case "w_queenBee": this.GetComponent<SpriteRenderer>().sprite = w_queenBee; break;
            case "w_ant": this.GetComponent<SpriteRenderer>().sprite = w_ant; break;
            case "w_beetle": this.GetComponent<SpriteRenderer>().sprite = w_beetle; break;
            case "w_grasshopper": this.GetComponent<SpriteRenderer>().sprite = w_grasshopper; break;
            case "w_spider": this.GetComponent<SpriteRenderer>().sprite = w_spider; break;
        }

    }

    public int GetXBoard() {  return this.xBoard; }
    public int GetYBoard() { return this.yBoard; }
    public void SetXBoard(int xBoard) { this.xBoard = xBoard; }
    public void SetYBoard(int yBoard) { this.yBoard = yBoard; }

    //to be edited...
    public void SetCoords()
    {
        float x = xBoard;
        float y = yBoard;
        x *= 0.75f; 
        
        if(xBoard%2 == 0)
        {
            y *= 1f;
        }
        else {
            y *= 0.5f;
        }
        

        x += -9.0f;
        y += -5.0f;
        this. transform.position = new Vector3(x, y, -1.0f);   //-z to be infront of the board not behind it

    }
}
