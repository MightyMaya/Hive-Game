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

    


    //function called when the move plate is created
    public void Start()
    {
        if (overlap) { 
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
