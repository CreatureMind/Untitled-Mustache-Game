using System;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Settings_Screen : Base_Menu
{
    [SerializeField] private Button musicToggleButton;
    [SerializeField] private Button sfxToggleButton;
    [SerializeField] private Button vibrationToggleButton;
    [SerializeField] private Button createProfileButton;
    [SerializeField] private Button loadProfileButton;

    [SerializeField] private Button aboutButton;
    [SerializeField] private Button backButton;

    [SerializeField] private Sprite selectedSprite;
    [SerializeField] private Sprite unselectedSprite;

    private Settings_Data _settingsData;

    private void Awake()
    {
        musicToggleButton.onClick.AddListener(() => ToggleMusic(!_settingsData.isMusicEnabled));
        sfxToggleButton.onClick.AddListener(() => ToggleSfx(!_settingsData.isSfxEnabled));
        vibrationToggleButton.onClick.AddListener(() => ToggleVibrations(!_settingsData.isVibrationsEnabled));
        aboutButton.onClick.AddListener(() => Menu_Manager.Instance.SwitchMenu(MenuState.About));
        backButton.onClick.AddListener(() => Menu_Manager.Instance.SwitchMenu(MenuState.Title));
        createProfileButton.onClick.AddListener(CreateNewProfile);
        loadProfileButton.onClick.AddListener(LoadProfile);
        
    }
    
    public override void Initialize()
    {
        _settingsData = Game_Manager.Instance.Settings;
        ToggleMusic(_settingsData.isMusicEnabled);
        ToggleSfx(_settingsData.isSfxEnabled);
        ToggleVibrations(_settingsData.isVibrationsEnabled);
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

    private void ToggleMusic(bool enable)
    {
        _settingsData.isMusicEnabled = enable;
        AudioManager.Instance.MuteMusic(enable);
        UpdateToggleButtons();
    }

    private void ToggleSfx(bool enable)
    {
        _settingsData.isSfxEnabled = enable;
        AudioManager.Instance.MuteSFX(enable);
        UpdateToggleButtons();
    }

    private void ToggleVibrations(bool enable)
    {
        _settingsData.isVibrationsEnabled = enable;
        if (enable)
        {
            Vibration_Manager.Instance.EnableVibration();
        }
        else
        {
            Vibration_Manager.Instance.DisableVibration();
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
        musicToggleButton.image.sprite = _settingsData.isMusicEnabled ? unselectedSprite : selectedSprite;
        sfxToggleButton.image.sprite = _settingsData.isSfxEnabled ? unselectedSprite :selectedSprite;
        vibrationToggleButton.image.sprite = _settingsData.isVibrationsEnabled ? unselectedSprite : selectedSprite;
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