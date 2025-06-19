using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Settings_Screen : Base_Menu
{
    private string fileName = "Settings_Screen.json";
    [SerializeField] private Button musicToggleButton;
    [SerializeField] private Button sfxToggleButton;
    [SerializeField] private Button darkModeToggleButton;
    [SerializeField] private Button unlockAllButton;
    [SerializeField] private Button resetGameDataButton;

    [SerializeField] private Button aboutButton;
    [SerializeField] private Button backButton;

    [SerializeField] private Sprite selectedSprite;
    [SerializeField] private Sprite unselectedSprite;

    private Settings_Data settingsData;

    private void Awake()
    {
        ReadJsonToSettingsData();
        musicToggleButton.onClick.AddListener(ToggleMusic);
        sfxToggleButton.onClick.AddListener(ToggleSfx);
        darkModeToggleButton.onClick.AddListener(ToggleDarkMode);
        aboutButton.onClick.AddListener(() => Menu_Manager.Instance.SwitchMenu(MenuState.About));
        backButton.onClick.AddListener(() => Menu_Manager.Instance.SwitchMenu(MenuState.Title));
        UpdateToggleButtons();
    }

    private void UnlockAllContent()
    {
        // Logic to unlock all content
    }

    private void ResetGameData()
    {
        // Logic to reset game data
    }

    private void ToggleMusic()
    {
        settingsData.IsMusicEnabled = !settingsData.IsMusicEnabled;
        UpdateToggleButtons();
        SaveSettingsData();
    }

    private void ToggleSfx()
    {
        settingsData.IsSfxEnabled = !settingsData.IsSfxEnabled;
        UpdateToggleButtons();
        SaveSettingsData();
    }

    private void ToggleDarkMode()
    {
        settingsData.IsDarkModeEnabled = !settingsData.IsDarkModeEnabled;
        UpdateToggleButtons();
        SaveSettingsData();
    }

    private void SaveSettingsData()
    {
        try
        {
            string json = JsonUtility.ToJson(settingsData, true);
            string directory = Application.persistentDataPath; // Better than StreamingAssets
            string path = Path.Combine(directory, fileName);

            Directory.CreateDirectory(directory); // Creates if doesn't exist
            File.WriteAllText(path, json);
            Debug.Log($"Settings saved to: {path}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save settings: {e.Message}");
        }
    }

    private void ReadJsonToSettingsData()
    {
        try
        {
            string path = Path.Combine(Application.persistentDataPath, fileName);

            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                Settings_Data loadedData = JsonUtility.FromJson<Settings_Data>(json);
                if (loadedData != null)
                {
                    settingsData = loadedData;
                    Debug.Log($"Settings loaded from: {path}");
                }
                else
                {
                    // Default values
                    settingsData = new Settings_Data
                    {
                        IsMusicEnabled = true,
                        IsSfxEnabled = true,
                        IsDarkModeEnabled = false
                    };
                    Debug.LogWarning("Settings data is null, using default values.");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to read settings: {e.Message}");
        }
    }
    private void UpdateToggleButtons()
    {
        Debug.Log($"Music: {settingsData.IsMusicEnabled}, SFX: {settingsData.IsSfxEnabled}, Dark Mode: {settingsData.IsDarkModeEnabled}");
        musicToggleButton.image.sprite = settingsData.IsMusicEnabled ? selectedSprite : unselectedSprite;
        sfxToggleButton.image.sprite = settingsData.IsSfxEnabled ? selectedSprite : unselectedSprite;
        darkModeToggleButton.image.sprite = settingsData.IsDarkModeEnabled ? selectedSprite : unselectedSprite;
    }

}

public class Settings_Data
{
    public bool IsMusicEnabled { get; set; }
    public bool IsSfxEnabled { get; set; }
    public bool IsDarkModeEnabled { get; set; }
}