using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Game_Manager : MonoBehaviour
{
    public static Game_Manager Instance { get; private set; }
    public Profile_Data Profile { get; set; }
    
    //should be data list
    
    private List<Progress_Data> _progress;
    public List<Progress_Data> Progress
    {
        get
        {
            if (Profile == null || string.IsNullOrEmpty(Profile.progressPath))
                return new List<Progress_Data>(); // Return empty list if no profile or progress path

            var progress = JsonHelper.LoadList<Progress_Data>(Profile.progressPath);
            return progress ?? new List<Progress_Data>(); // Return empty list if loading fails
        }
    }
    
    private Settings_Data _settings;
    public Settings_Data Settings 
    {
        get
        {
            if (Profile == null || string.IsNullOrEmpty(Profile.settingsPath))
                return new Settings_Data(); // Return default settings if no profile or settings path

            var settings = JsonHelper.Load<Settings_Data>(Profile.settingsPath);
            return settings ?? new Settings_Data(); // Return default if loading fails
        }
    }

    private void Awake()
    {
        Level_Manager.RoundScoreCalculated += EditProgressToSave;
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Profile_Menu.ProfilesPath = Path.Join(Application.persistentDataPath, "/Profiles");
        
        if(!CheckFirstTime())
            LoadLastUsedProfile();
    }

    public static bool CheckFirstTime()
    {
        // If the Profiles folder doesn't exist, or it has no subdirectories, assume first time
        return !Directory.Exists(Profile_Menu.ProfilesPath) || Directory.GetDirectories(Profile_Menu.ProfilesPath).Length == 0;
    }

    private void LoadLastUsedProfile()
    {
        var nickname = PlayerPrefs.GetString("LastProfile", null);
        if (string.IsNullOrEmpty(nickname)) return;

        var profile = Profile_Menu.LoadProfileByNickname(nickname);
        if (profile == null) return;

        Profile = profile;
        _progress = JsonHelper.LoadList<Progress_Data>(profile.progressPath);
        _settings = JsonHelper.Load<Settings_Data>(profile.settingsPath);

        Profile_Menu.ActiveProfile = Profile;
        Debug.Log(Profile);
        
    }
    
    private void EditProgressToSave(int index, int starsEarned)
    {
        if (Profile == null || string.IsNullOrEmpty(Profile.progressPath)) return;

        // Find the progress entry for the given level index
        var progressEntry = _progress.Find(p => p.levelIndex == index);
        if (progressEntry == null)
        {
            // If not found, create a new entry
            progressEntry = new Progress_Data { levelIndex = index, starsEarned = 0 };
            _progress.Add(progressEntry);
        }

        // Update the stars earned
        progressEntry.starsEarned = Mathf.Max(progressEntry.starsEarned, starsEarned);
        
        SaveProgress(_progress);
    }

    public void SaveProgress(List<Progress_Data> progress)
    {
        if (Profile == null || string.IsNullOrEmpty(Profile.progressPath)) return;
        _progress = progress;
        JsonHelper.SaveList(Profile.progressPath, _progress);
    }

    public void SaveSettings(Settings_Data settingsData)
    {
        if (Profile == null || string.IsNullOrEmpty(Profile.settingsPath)) return;
        _settings = settingsData;
        JsonHelper.Save(Profile.settingsPath, _settings);
    }

    public void SwitchProfile(Profile_Data newProfile)
    {
        Profile = newProfile;
        _progress = JsonHelper.LoadList<Progress_Data>(Profile.progressPath);
        _settings = JsonHelper.Load<Settings_Data>(Profile.settingsPath);
        PlayerPrefs.SetString("LastProfile", newProfile.nickname);
        PlayerPrefs.Save();
        
        Profile_Menu.ActiveProfile = Profile;
        
        // SettingsManager.Apply(Settings);
        // LevelManager.Reload(); // or a scene reload if needed
        // UIManager.Refresh(); // replace with your actual UI update logic
    }
}
