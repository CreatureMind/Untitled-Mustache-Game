using System;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Settings_Screen : Base_Menu
{
    private string fileName = "Settings_Screen.json";
    [SerializeField] private Button musicToggleButton;
    [SerializeField] private Button sfxToggleButton;
    [SerializeField] private Button darkModeToggleButton;
    [SerializeField] private Button unlockAllButton;
    [SerializeField] private Button resetGameDataButton;
    [SerializeField] private Button createProfileButton;
    [SerializeField] private Button loadProfileButton;

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
        createProfileButton.onClick.AddListener(CreateNewProfile);
        loadProfileButton.onClick.AddListener(LoadProfile);
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
        settingsData.isMusicEnabled = !settingsData.isMusicEnabled;
        AudioManager.Instance.MuteMusic(settingsData.isMusicEnabled);
        UpdateToggleButtons();
    }

    private void ToggleSfx()
    {
        settingsData.isSfxEnabled = !settingsData.isSfxEnabled;
        AudioManager.Instance.MuteSFX(settingsData.isSfxEnabled);
        UpdateToggleButtons();
    }

    private void ToggleDarkMode()
    {
        settingsData.isDarkModeEnabled = !settingsData.isDarkModeEnabled;
        UpdateToggleButtons();
    }

    private void CreateNewProfile()
    {
        Profile_Menu.CurrentProfileState = Profile_State.Creating;
        Menu_Manager.Instance.SwitchMenu(MenuState.Profile);
    }
    
    private void LoadProfile()
    {
        Profile_Menu.CurrentProfileState = Profile_State.Loading;
        Menu_Manager.Instance.SwitchMenu(MenuState.Profile);
    }

    private void SaveSettingsData()
    {
        string json = JsonUtility.ToJson(settingsData, true);
        string directory = Application.streamingAssetsPath;
        string path = Path.Combine(directory, fileName);

        Directory.CreateDirectory(directory);
        File.WriteAllText(path, json);
        Debug.Log($"Settings saved to: {path}");
    }

    private void ReadJsonToSettingsData()
    {
        string path = Path.Combine(Application.streamingAssetsPath, fileName);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            Debug.Log($"Settings: {json}");
            Settings_Data loadedData = JsonUtility.FromJson<Settings_Data>(json);
            settingsData = loadedData ?? CreateDefaultSettings();
        }
        else
        {
            settingsData = CreateDefaultSettings();
            SaveSettingsData();
        }
    }

    private Settings_Data CreateDefaultSettings()
    {
        return new Settings_Data
        {
            isMusicEnabled = true,
            isSfxEnabled = true,
            isDarkModeEnabled = false
        };
    }
    private void UpdateToggleButtons()
    {
        musicToggleButton.image.sprite = settingsData.isMusicEnabled ? selectedSprite : unselectedSprite;
        sfxToggleButton.image.sprite = settingsData.isSfxEnabled ? selectedSprite : unselectedSprite;
        darkModeToggleButton.image.sprite = settingsData.isDarkModeEnabled ? selectedSprite : unselectedSprite;
    }

    private void OnDisable()
    {
        SaveSettingsData();
    }
}

[Serializable]
public class Settings_Data
{
    public bool isMusicEnabled;
    public bool isSfxEnabled;
    public bool isDarkModeEnabled;
}