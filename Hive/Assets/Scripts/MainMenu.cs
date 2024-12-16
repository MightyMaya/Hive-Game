using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button humanVsHumanButton; // Assigned in Inspector
    [SerializeField] private Button humanVsAIButton;    // Assigned in Inspector
    [SerializeField] private Button aiVsAIButton;      // Assigned in Inspector

    [SerializeField] private TMP_Dropdown dd1;         // Assigned in Inspector
    [SerializeField] private TMP_Dropdown dd2;         // Assigned in Inspector
    [SerializeField] private GameObject optionsPanel;  // Assigned in Inspector (e.g., OptionsPanel)

    private void OnEnable()
    {
        // Subscribe to sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Attach UI listeners for the current scene
        AttachListeners();
    }

    private void OnDisable()
    {
        // Unsubscribe from sceneLoaded event
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        // Ensure UI listeners are attached on start
        AttachListeners();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Ensure UI listeners are re-attached after the scene is loaded
        StartCoroutine(AttachListenersNextFrame());
    }

    private IEnumerator AttachListenersNextFrame()
    {
        // Wait for the next frame to ensure UI is fully initialized
        yield return null;
        AttachListeners();
    }

    private void AttachListeners()
    {
        if (optionsPanel == null)
        {
            Debug.LogError("OptionsPanel is not assigned in the Inspector!");
            return;
        }

        // Enable the options panel temporarily if needed
        if (!optionsPanel.activeInHierarchy)
        {
            optionsPanel.SetActive(true);
        }

        // Attach button listeners
        if (humanVsHumanButton != null)
        {
            humanVsHumanButton.onClick.RemoveAllListeners();
            humanVsHumanButton.onClick.AddListener(OnHumanTwoButtonClick);
        }

        if (humanVsAIButton != null)
        {
            humanVsAIButton.onClick.RemoveAllListeners();
            humanVsAIButton.onClick.AddListener(OnHumanAIButtonClick);
        }

        if (aiVsAIButton != null)
        {
            aiVsAIButton.onClick.RemoveAllListeners();
            aiVsAIButton.onClick.AddListener(OnAITwoButtonClick);
        }

        // Attach dropdown listeners
        if (dd1 != null)
        {
            dd1.onValueChanged.RemoveAllListeners();
            dd1.onValueChanged.AddListener(index => OnDifficulty1Changed(index));
        }

        if (dd2 != null)
        {
            dd2.onValueChanged.RemoveAllListeners();
            dd2.onValueChanged.AddListener(index => OnDifficulty2Changed(index));
        }

        // Re-disable the options panel after attaching listeners
        if (optionsPanel.activeInHierarchy)
        {
            optionsPanel.SetActive(false);
        }
    }

    // Button Click Handlers
    public void OnHumanTwoButtonClick()
    {
        if (GameSettings.Instance != null)
        {
            GameSettings.Instance.currentMode = GameSettings.GameMode.HumanVsHuman;
        }
        SceneManager.LoadScene("Game");
    }

    public void OnHumanAIButtonClick()
    {
        if (GameSettings.Instance != null)
        {
            GameSettings.Instance.currentMode = GameSettings.GameMode.HumanVsAI;
        }
        SceneManager.LoadScene("Game");
    }

    public void OnAITwoButtonClick()
    {
        if (GameSettings.Instance != null)
        {
            GameSettings.Instance.currentMode = GameSettings.GameMode.AIvsAI;
        }
        SceneManager.LoadScene("Game");
    }

    public void OnQuitButtonClick()
    {
        
        Application.Quit();
    }
    // Dropdown Value Changed Handlers
    public void OnDifficulty1Changed(int index)
    {
        if (GameSettings.Instance != null)
        {
            GameSettings.Instance.aiDifficulty1 = MapDropdownIndexToDifficulty(index);
        }
        Debug.Log($"AI Difficulty 1 set to: {GameSettings.Instance.aiDifficulty1}");
    }

    public void OnDifficulty2Changed(int index)
    {
        if (GameSettings.Instance != null)
        {
            GameSettings.Instance.aiDifficulty2 = MapDropdownIndexToDifficulty(index);
        }
        Debug.Log($"AI Difficulty 2 set to: {GameSettings.Instance.aiDifficulty2}");
    }

    // Map Dropdown Index to Difficulty Enum
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
