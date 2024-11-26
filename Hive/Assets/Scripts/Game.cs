using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public GameObject piece;
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(piece, new Vector3(0, 0, -1), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
