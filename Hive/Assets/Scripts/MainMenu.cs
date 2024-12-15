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
        if (GameSettings.Instance != null)
        {
            GameSettings.Instance.currentMode = GameSettings.GameMode.HumanVsHuman;
        }

        // Load the scene
        SceneManager.LoadScene("Game");
    }

    // This function will be called when the Human Vs. AI button is clicked
    public void OnHumanAIButtonClick()
    {
        if (GameSettings.Instance != null)
        {
            GameSettings.Instance.currentMode = GameSettings.GameMode.HumanVsAI;
            Debug.Log("Game mode is HumanvsAI");
        }

        // load the scene
        SceneManager.LoadScene("Game");
    }

    // This function will be called when the AI Vs. AI button is clicked
    public void OnAITwoButtonClick()
    {
        if (GameSettings.Instance != null)
        {
            GameSettings.Instance.currentMode = GameSettings.GameMode.AIvsAI;
        }
        // load the scene
        SceneManager.LoadScene("Game");
    }
}
