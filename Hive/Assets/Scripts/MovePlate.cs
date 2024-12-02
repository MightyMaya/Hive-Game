using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlate : MonoBehaviour
{
    public GameObject controller;

    //referemce to the piece that created the moveplate
    GameObject reference = null;

    //Board positions
    int matrixX;
    int matrixY;

    //is it going to overlap over another piece
    public bool overlap = false;

    //is it going to break the hive
    public bool hiveBreak = false;


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
        controller = GameObject.FindGameObjectWithTag("GameController");

        if (!hiveBreak)
        {
            //set old position of the piece to be empty
            controller.GetComponent<Game>().SetPositionEmpty(reference.GetComponent<Hiveman>().GetXBoard(),
                reference.GetComponent<Hiveman>().GetYBoard());

            //set new position of the piece to the clicked position
            reference.GetComponent<Hiveman>().SetXBoard(matrixX);
            reference.GetComponent<Hiveman>().SetYBoard(matrixY);
            reference.GetComponent<Hiveman>().SetCoords();

            //tell the game controller where the piece has moved
            controller.GetComponent<Game>().SetPosition(reference);

            //destroy the moveplates made
            reference.GetComponent<Hiveman>().DestroyMovePlates();

        }
        else { 
            //stop piece from moving
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
