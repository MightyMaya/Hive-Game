using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Game : MonoBehaviour
{
    public GameObject hivepiece;


    //Positions and team for each hivepiece
    //positions array , size ?????
    private GameObject[,] positions = new GameObject [29,12];
    private GameObject[] blackPlayer = new GameObject[9];
    private GameObject[] whitePlayer = new GameObject[9];

    private string currentPlayer = "white";

    private bool gameOver = false;
    
    // Start is called before the first frame update
    void Start()
    {

        whitePlayer = new GameObject[]
        {
            Create("w_queenBee", 28, 1),
            Create("w_ant", 28, 2),
            Create("w_ant", 28, 3),
            Create("w_beetle", 28, 4),
            Create("w_beetle", 28, 5),
            Create("w_spider", 28, 6),
            Create("w_spider", 28, 7),
            Create("w_grasshopper", 28, 8),
            Create("w_grasshopper", 28, 9)
        };
          
        blackPlayer = new GameObject[]
        {
            Create("b_queenBee", 0, 1),
            Create("b_ant", 0, 2),
            Create("b_ant", 0, 3),
            Create("b_beetle", 0, 4),
            Create("b_beetle", 0, 5),
            Create("b_spider", 0, 6),
            Create("b_spider", 0, 7),
            Create("b_grasshopper", 0, 8),
            Create("b_grasshopper", 0, 9)

    };

        //Set all piece positions on the position board
        for (int i = 0; i < whitePlayer.Length; i++) {
            SetPosition(blackPlayer[i]);
            SetPosition(whitePlayer[i]);
        }

    }
   /* public bool IsFirstMove()
    {
        return isFirstMove;
    }

    public void EndFirstMove()
    {
        isFirstMove = false;
    }*/

    public bool IsOnBoard(int x, int y)
    {

        // Default check for other moves
        return x >= 0 && y >= 0 && x < 29 && y < 12;
    }



    public GameObject Create (string name, int x, int y)
    {
        GameObject obj = Instantiate(hivepiece, new Vector3(0,0,-1), Quaternion.identity);
        Hiveman hm = obj.GetComponent<Hiveman>();
        hm.name = name;
        hm.SetXBoard(x);
        hm.SetYBoard(y);
        hm.Activate();  //render the sprite
        return obj;
    }

    public void SetPosition(GameObject obj)
    {
        Hiveman hm = (obj.GetComponent<Hiveman>());
        positions[hm.GetXBoard(), hm.GetYBoard()] = obj;
    }

    //set empty position to null after we move a piece
    public void SetPositionEmpty(int x, int y)
    {
        positions[x, y] = null;
    }

    //get object at certain position
    public GameObject GetPosition(int x, int y)
    {
        return positions[x, y];
    }

  

  
}
