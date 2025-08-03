using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Profile_Menu : Base_Menu
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button createButton;
    [SerializeField] private Button backButton;
    [SerializeField] private TMP_InputField nicknameText;
    [SerializeField] private List<Transform> profilePanels;
    public static Profile_State CurrentProfileState { get; set; } = Profile_State.New;
    //public static bool IsFirstTime {get; internal set;}
    public static Profile_Data ActiveProfile { get; internal set; }
    public static string ProfilesPath { get; internal set;}

    private void Awake()
    {
        if(Game_Manager.CheckFirstTime())
            profilePanels[0].gameObject.SetActive(false);

        playButton.onClick.AddListener(CreateNewProfile);
        playButton.interactable = false;
        createButton.onClick.AddListener(CreateNewProfile);
        createButton.interactable = false;
        backButton.onClick.AddListener(() => Menu_Manager.Instance.SwitchMenu(MenuState.Settings));
    }

    private void Update()
    {
        if (!profilePanels[0].gameObject.activeSelf && !profilePanels[1].gameObject.activeSelf) return;
        if (!string.IsNullOrEmpty(nicknameText.text)) return;
        playButton.interactable = true;
        createButton.interactable = true;
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
    
    private void CreateNewProfile()
    {
        if (string.IsNullOrEmpty(nicknameText.text))
        {
#if UNITY_EDITOR

            Debug.LogError("Name cannot be null or empty");
#endif
            return;
        }
        
        var nickname = nicknameText.text;
        var profilesDirPath = Path.Join(ProfilesPath, "/", nickname);
        var path = Path.Join(profilesDirPath, "/profile.json");

        // Ensure directories exist
        Directory.CreateDirectory(profilesDirPath);
        
        var profile = new Profile_Data
        {
            nickname = nickname,
            character = "Chick",
            totalStarsEarned = 0,
            progressPath = CreateProgressJson(profilesDirPath),
            settingsPath = CreateSettingJson(profilesDirPath)
        };

        JsonHelper.Save(path, profile);
        Debug.Log($"Profile saved to: {path}");
        
        PlayerPrefs.SetString("LastProfile", nickname);
        PlayerPrefs.Save();
        
        ActiveProfile = profile;
        
        Menu_Manager.Instance.SwitchMenu(MenuState.Title);
    }
    private string CreateProgressJson(string profilesPath)
    {
        var path = Path.Join(profilesPath, "/progress.json");
        var progress = new List<Progress_Data>
        {
            new Progress_Data { levelIndex = 0, starsEarned = 0 },
        };
        
        JsonHelper.SaveList(path, progress);
        
        return path;
    }
    private string CreateSettingJson(string profilesPath)
    {
        var path = Path.Join(profilesPath, "/settings.json");

        JsonHelper.Save(path, new Settings_Data());
        
        return path;
    }
    
    public static Profile_Data LoadProfileByNickname(string nickname)
    {
        var profileDir = Path.Combine(ProfilesPath, nickname);
        var profilePath = Path.Combine(profileDir, "profile.json");

        return JsonHelper.Load<Profile_Data>(profilePath);
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

    public override string ToString()
    {
        return $"Nickname: {nickname} | Character: {character} | Total Stars Earned: {totalStarsEarned}";
    }
}

[Serializable]
public class Progress_Data
{
    public int levelIndex; //check this if logic breaks
    public int starsEarned;
}

public enum Profile_State
{
    New,
    Creating,
    Loading
}