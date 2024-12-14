using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
   //add variable to keep track of AIPlayer number

    //add variable to control difficulty


    // This function will be called when the options button is clicked
    public void OnOptionButtonClick()
    {
        Debug.Log("Option button clicked!");
        
    }

    // This function will be called when the Human Vs. Human button is clicked
    public void OnHumanTwoButtonClick()
    {
        //ensure AI players are deactivated

        // load the scene
        SceneManager.LoadScene("Game");
    }

    // This function will be called when the Human Vs. AI button is clicked
    public void OnHumanAIButtonClick()
    {
        //ensure 1 AI player is activated

        // load the scene
        SceneManager.LoadScene("Game");
    }

    // This function will be called when the AI Vs. AI button is clicked
    public void OnAITwoButtonClick()
    {
        //ensure both AI playera are activated

        // load the scene
        SceneManager.LoadScene("Game");
    }
}
