using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    //add variable to keep track of AIPlayer number

    //add variable to control difficulty

    //get reference to first dropdown menu
    [SerializeField] private TMP_Dropdown dd1;

    //get reference to second dropdown menu
    [SerializeField] private TMP_Dropdown dd2;
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
    void Start()
    {
        // Set up listeners for dropdowns
        if (dd1 != null)
        {
            dd1.onValueChanged.AddListener(delegate { OnDifficulty1Changed(dd1.value); });
        }

        if (dd2 != null)
        {
            dd2.onValueChanged.AddListener(delegate { OnDifficulty2Changed(dd2.value); });
        }
    }

    // Called when dropdown 1 changes
    public void OnDifficulty1Changed(int index)
    {
        if (GameSettings.Instance != null)
        {
            // Map dropdown index to Difficulty
            GameSettings.Instance.aiDifficulty1 = MapDropdownIndexToDifficulty(index);
        }
        Debug.Log($"AI Difficulty 1 set to: {GameSettings.Instance.aiDifficulty1}");
    }

    public void OnDifficulty2Changed(int index)
    {
        if (GameSettings.Instance != null)
        {
            // Map dropdown index to Difficulty
            GameSettings.Instance.aiDifficulty2 = MapDropdownIndexToDifficulty(index);
        }
        Debug.Log($"AI Difficulty 2 set to: {GameSettings.Instance.aiDifficulty2}");
    }

    private GameSettings.Difficulty MapDropdownIndexToDifficulty(int index)
    {
        switch (index)
        {
            case 0: return GameSettings.Difficulty.Easy;
            case 1: return GameSettings.Difficulty.Medium;
            case 2: return GameSettings.Difficulty.Hard;
            default: return GameSettings.Difficulty.Medium;
        }
    }

}
