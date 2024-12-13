using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Hiveman;

public class PlayerAI : MonoBehaviour
{
    public GameObject controller;

    private GameObject b_queen;
    
    private string aiPlayer = "b";

    private Game gamesc;

    private Hiveman b_queen_controller;

    private MovePlate b_queen_moveplate;

    // Flag to ensure OnMouseUp is called only once
    private bool actionTriggered = false;

    //list to store all the black pieces
    private List<GameObject> b_pieces;

    //list to store all the move scripts
    private List<IMoveLogic> moveScripts;
    private void Start()
    {
        // Get a reference to game controller
        controller = GameObject.FindWithTag("GameController");
        gamesc = controller.GetComponent<Game>();

        moveScripts = new List<IMoveLogic>();

        // Find all GameObjects with the tag "Piece"
        GameObject[] allPieces = GameObject.FindGameObjectsWithTag("Piece");

        // Filter pieces whose names start with "b"
        b_pieces = new List<GameObject>();
        foreach (GameObject piece in allPieces)
        {
            if (piece.name.StartsWith("b"))
            {
                b_pieces.Add(piece);
            }
        }

        foreach (GameObject piece in b_pieces)
        {
            switch (piece.name)
            {
                case "b_queenBee":
                    moveScripts.Add(piece.GetComponent<QueenBeeMoves>());
                    break;
                case "b_ant":
                    moveScripts.Add(piece.GetComponent<AntMoves>());
                    break;
                case "b_beetle":
                    moveScripts.Add(piece.GetComponent<BeetleMoves>());
                    break;
                case "b_grasshopper":
                    moveScripts.Add(piece.GetComponent<GrassMoves>());
                    break;
                case "b_spider":
                    moveScripts.Add(piece.GetComponent<SpiderMoves>());
                    break;

                
            }
        }

        /*// Ensure there's at least one "b" piece
        if (b_pieces.Count > 0)
        {
            Debug.Log($"Found {b_pieces.Count} black  pieces");
        }*/

        // Ensure there's at least one move script
        if (b_pieces.Count > 0)
        {
            Debug.Log($"Found {moveScripts.Count} move scripts");
        }


    }

   /* private void Update()
    {
        if (!actionTriggered && gamesc.GetCurrentPlayer() == aiPlayer)
        {
            //get possible moves
            //b_queen_controller.OnMouseUp();
            //move the piece
           // b_queen_moveplate.OnMouseUp();
            actionTriggered = true; // Set the flag to true
        }
    }

    public void ResetAction()
    {
        // Call this method when you want to reset the flag
        actionTriggered = false;
    }
   */

}
