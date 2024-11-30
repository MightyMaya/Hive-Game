using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public GameObject hivepiece;


    //Positions and team for each hivepiece
    //positions array , size ?????
    
    private GameObject[] blackPlayer = new GameObject[9];
    private GameObject[] whitePlayer = new GameObject[9];

    private string currentPlayer = "white";

    private bool gameOver = false;

    // Start is called before the first frame update
    void Start()
    {
        whitePlayer = new GameObject[]
        {
            //adjust the 9 pieces initial position
            //Create("")
        };

        blackPlayer = new GameObject[]
        {
            //adjust the 9 pieces initial position
            //Create("")
        };

        //Set all piece positions on the position board


    }



    // Update is called once per frame
    void Update()
    {
    }
}
