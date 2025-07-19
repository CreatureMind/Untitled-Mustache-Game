using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Profile_Menu : Base_Menu
{
    public static bool IsFirstTime {get; private set;}
    [SerializeField] private Button playButton;
    [SerializeField] private Button backButton;
    [SerializeField] private TMP_InputField nicknameText;
    [SerializeField] private List<Transform> profilePanels;
    public static Profile_State CurrentProfileState { get; set; } = Profile_State.New;

    private void Awake()
    {
        // Check if this is the first launch
        CheckFirstTime();

        playButton.onClick.AddListener(CreateNewProfile);
        playButton.interactable = false;
        backButton.onClick.AddListener(() => Menu_Manager.Instance.SwitchMenu(MenuState.Settings));
    }

    private void Update()
    {
        if (string.IsNullOrEmpty(nicknameText.text))
        {
            playButton.interactable = true;
        }
    }

    protected override void OnMenuOpen()
    {
        switch (CurrentProfileState)
        {
            case Profile_State.New:
                profilePanels[0].gameObject.SetActive(true);
                nicknameText.gameObject.SetActive(true);
                backButton.gameObject.SetActive(false);
                break;
            case Profile_State.Creating:
                profilePanels[1].gameObject.SetActive(true);
                nicknameText.gameObject.SetActive(true);
                backButton.gameObject.SetActive(true);
                break;
            case Profile_State.Loading:
            default:
                profilePanels[2].gameObject.SetActive(true);
                backButton.gameObject.SetActive(true);
                break;
        }
    }

    protected override void OnMenuClose()
    {
        profilePanels.ForEach(p => p.gameObject.SetActive(false));
        
        nicknameText.gameObject.SetActive(false);
        playButton.interactable = false;
        nicknameText.text = "";
        CurrentProfileState = Profile_State.New;
    }
    
    private void CheckFirstTime()
    {
        var profilesPath = Path.Combine(Application.persistentDataPath, "Profiles");

        // If the Profiles folder doesn't exist, or it has no subdirectories, assume first time
        if (!Directory.Exists(profilesPath) || Directory.GetDirectories(profilesPath).Length == 0)
        {
            IsFirstTime = true;
        }
        else
        {
            IsFirstTime = false;
            profilePanels[0].gameObject.SetActive(false);
        }
    }
    
    private void CreateNewProfile()
    {
        if (string.IsNullOrEmpty(nicknameText.text))
        {
            Debug.LogError("Name cannot be null or empty");
            return;
        }
        
        var nickname = nicknameText.text;
        var profilesPath = Path.Combine(Application.persistentDataPath, "Profiles", nickname);
        var path = Path.Combine(profilesPath, "profile.json");

        // Ensure directories exist
        Directory.CreateDirectory(profilesPath);
        
        var profile = new Profile_Data
        {
            nickname = nickname,
            character = "Default",
            totalStarsEarned = 0,
            progressPath = CreateProgressJson(profilesPath),
            settingsPath = CreateSettingJson(profilesPath)
        };
        
        var json = JsonUtility.ToJson(profile, true);
        File.WriteAllText(path, json);
        Debug.Log($"Profile saved to: {path}");
        
        Menu_Manager.Instance.SwitchMenu(MenuState.Title);
    }
    private string CreateProgressJson(string profilesPath)
    {
        var path = Path.Combine(profilesPath, "progress.json");
        
        var json = JsonUtility.ToJson(new Progress_Data(), true);
        File.WriteAllText(path, json);
        
        return path;
    }
    private string CreateSettingJson(string profilesPath)
    {
        var path = Path.Combine(profilesPath, "settings.json");

        var json = JsonUtility.ToJson(new Settings_Data(), true);
        File.WriteAllText(path, json);
        
        return path;
    }
}

[Serializable]
public class Profile_Data
{
    public string nickname;
    public string character;
    public int totalStarsEarned;
    public string progressPath;
    public string settingsPath;
}

[Serializable]
public class Progress_Data
{
    public string levelName;
    public int starsEarned;
}

public enum Profile_State
{
    New,
    Creating,
    Loading
}