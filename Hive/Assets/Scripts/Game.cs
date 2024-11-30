using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public GameObject hivepiece;


    //Positions and team for each hivepiece
    //positions array , size ?????
    private GameObject[,] positions = new GameObject [11,27];
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

    public GameObject Create (string name, int x, int y)
    {
        GameObject obj = Instantiate(hivepiece, new Vector3(0,0,-1), Quaternion.identity);
        Hiveman hm = obj.GetComponent<Hiveman>();
        hm.name = name;
        hm.SetXBoard(x);
        hm.setYBoard(y);
        hm.Activate();  //render the sprite
        return obj;
    }

    public void SetPosition(GameObject obj)
    {
        Hiveman hm = (obj.GetComponent<Hiveman>());
        positions[hm.GetXBoard(), hm.GetYBoard()] = obj;
    }


    // Update is called once per frame
    void Update()
    {
    }
}
