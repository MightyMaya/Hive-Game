using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameSettings : MonoBehaviour
{
   
    public static GameSettings Instance { get; private set; }

    public enum GameMode
    {
        HumanVsHuman,
        HumanVsAI,
        AIvsAI
    }
    public enum Difficulty
    {
        Easy = 2, // Shallow depth
        Medium = 4, // Moderate depth
        Hard = 6 // Deep search
    }

    public GameMode currentMode;
    //Medium by default
    public Difficulty aiDifficulty1 = Difficulty.Medium;
    //Medium by default
    public Difficulty aiDifficulty2 = Difficulty.Medium;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void ResetState()
    {
        currentMode = GameMode.HumanVsHuman;
        aiDifficulty1 = Difficulty.Medium;
        aiDifficulty2 = Difficulty.Medium;
    }
}
