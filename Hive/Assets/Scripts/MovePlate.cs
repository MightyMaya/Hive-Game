using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlate : MonoBehaviour
{
    public GameObject Controller;

    //referemce to the piece that created the moveplate
    GameObject reference = null;

    //Board positions
    int matrixX;
    int matrixY;

    //is it going to overlap over another piece
    public bool overlap = false;

    // Highlight center during the first move
    public bool isFirstMove = false;


    //function called when the move plate is created
    public void Start()
    {
        if(isFirstMove)
        {
            // Highlight the center with a special color for the first move
            gameObject.GetComponent<SpriteRenderer>().color = Color.yellow; // You can choose the color here
        }
        else if (overlap) { 
            //change to green
            gameObject.GetComponent<SpriteRenderer>().color = Color.green;
        }
    }

    //when moveplate is clicked
    public void OnMouseUp()
    {
        //get the game controller object
        Controller = GameObject.FindGameObjectWithTag("GameController");
        Game gamesc = Controller.GetComponent<Game>();

        Hiveman hivesc = reference.GetComponent<Hiveman>();
        if (!hivesc.hiveBreak)
        {
            //set old position of the piece to be empty
            gamesc.SetPositionEmpty(hivesc.GetXBoard(),hivesc.GetYBoard());

            //set new position of the piece to the clicked position
            hivesc.SetXBoard(matrixX);
            hivesc.SetYBoard(matrixY);
            hivesc.SetCoords();

            //tell the game controller where the piece has moved
            gamesc.SetPosition(reference);

            //piece is no longer in its first move
            //hivesc.SetFirstMove(false);


            // If this was the first move, toggle the flag
            if (gamesc.isFirstMove)
            {
                gamesc.isFirstMove = false;
                Debug.Log("First move completed.");
            }

         

            // Record the move           
            gamesc.RecordPlayerMove(gamesc.GetCurrentPlayer(), hivesc.name, hivesc.GetXBoard(), hivesc.GetYBoard()); // Record current position as the move



            //switch the player
            gamesc.NextTurn();
            //destroy the moveplates made
            hivesc.DestroyMovePlates();

        }
        else {
            //stop piece from moving
            Debug.Log("This move will break the hive");
        }
    }

    public void SetCoords(int x, int y)
    {
        matrixX = x;
        matrixY = y;    
    }

    public void SetReference(GameObject obj)
    {
        reference = obj;
    }

    public GameObject GetReference() {
        return reference;
    }
}
