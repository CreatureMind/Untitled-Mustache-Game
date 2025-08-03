using System;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Settings_Screen : Base_Menu
{
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

    private Settings_Data _settingsData;

    private void Awake()
    {
        musicToggleButton.onClick.AddListener(ToggleMusic);
        sfxToggleButton.onClick.AddListener(ToggleSfx);
        darkModeToggleButton.onClick.AddListener(ToggleVibrations);
        aboutButton.onClick.AddListener(() => Menu_Manager.Instance.SwitchMenu(MenuState.About));
        backButton.onClick.AddListener(() => Menu_Manager.Instance.SwitchMenu(MenuState.Title));
        createProfileButton.onClick.AddListener(CreateNewProfile);
        loadProfileButton.onClick.AddListener(LoadProfile);
    }

    private void UnlockAllContent()
    {
        // Logic to unlock all content
    }

    private void ResetGameData()
    {
        // Logic to reset game data
    }
    
    public override void Initialize()
    {
        _settingsData = Game_Manager.Instance.Settings;
    }
    
    protected override void OnMenuOpen()
    {
        UpdateToggleButtons();
    }

    protected override void OnMenuClose()
    {
        if (_settingsData == null) return;
        Game_Manager.Instance.SaveSettings(_settingsData);
    }

    private void ToggleMusic()
    {
        _settingsData.isMusicEnabled = !_settingsData.isMusicEnabled;
        AudioManager.Instance.MuteMusic(_settingsData.isMusicEnabled);
        UpdateToggleButtons();
    }

    private void ToggleSfx()
    {
        _settingsData.isSfxEnabled = !_settingsData.isSfxEnabled;
        AudioManager.Instance.MuteSFX(_settingsData.isSfxEnabled);
        UpdateToggleButtons();
    }

    private void ToggleVibrations()
    {
        _settingsData.isVibrationsEnabled = !_settingsData.isVibrationsEnabled;
        
        if (_settingsData.isVibrationsEnabled)
        {
            Vibration_Manager.Instance.DisableVibration();
        }
        else
        {
            Vibration_Manager.Instance.EnableVibration();
        }
        
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

    private void UpdateToggleButtons()
    {
        musicToggleButton.image.sprite = _settingsData.isMusicEnabled ? selectedSprite : unselectedSprite;
        sfxToggleButton.image.sprite = _settingsData.isSfxEnabled ? selectedSprite : unselectedSprite;
        darkModeToggleButton.image.sprite = _settingsData.isVibrationsEnabled ? selectedSprite : unselectedSprite;
    }
}

[Serializable]
public class Settings_Data
{
    public bool isMusicEnabled;
    public bool isSfxEnabled;
    public bool isVibrationsEnabled;
    
    
    public Settings_Data()
    {
        isMusicEnabled = true;
        isSfxEnabled = true;
        isVibrationsEnabled = true;
    }
}