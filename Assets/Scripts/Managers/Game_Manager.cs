using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Game_Manager : MonoBehaviour
{
    public static Game_Manager Instance { get; private set; }

    public Profile_Data Profile { get; private set; }
    
    //should be data list
    public List<Progress_Data> Progress { get; set; }
    public Settings_Data Settings { get; set; }

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
        Progress = JsonHelper.LoadList<Progress_Data>(profile.progressPath);
        Settings = JsonHelper.Load<Settings_Data>(profile.settingsPath);

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
        
        if (Profile_Menu.ActiveProfile != null)
        {
            Profile_Menu.ActiveProfile.totalStarsEarned = 0;
            foreach (var progress in _progress)
            {
                Profile.totalStarsEarned += progress.starsEarned;
            }
            SaveProfile(Profile);
        }
    }

    private void SaveProfile(Profile_Data profile)
    {
        if (profile == null || string.IsNullOrEmpty(profile.nickname)) return;

        var profilePath = Path.Combine(Profile_Menu.ProfilesPath, profile.nickname, "profile.json");
        JsonHelper.Save(profilePath, profile);
        Debug.Log($"Profile saved to: {profilePath}");
        
        PlayerPrefs.SetString("LastProfile", profile.nickname);
        PlayerPrefs.Save();
    }

    public void SaveProgress()
    {
        if (Profile == null || string.IsNullOrEmpty(Profile.progressPath)) return;
        JsonHelper.SaveList(Profile.progressPath, Progress);
    }

    public void SaveSettings(Settings_Data settingsData)
    {
        if (Profile == null || string.IsNullOrEmpty(Profile.settingsPath)) return;
        Settings = settingsData;
        JsonHelper.Save(Profile.settingsPath, Settings);
    }

    public void SwitchProfile(Profile_Data newProfile)
    {
        Profile = newProfile;
        Progress = JsonHelper.LoadList<Progress_Data>(Profile.progressPath);
        Settings = JsonHelper.Load<Settings_Data>(Profile.settingsPath);
        PlayerPrefs.SetString("LastProfile", newProfile.nickname);
        PlayerPrefs.Save();
        
        Profile_Menu.ActiveProfile = Profile;
        
        // SettingsManager.Apply(Settings);
        // LevelManager.Reload(); // or a scene reload if needed
        // UIManager.Refresh(); // replace with your actual UI update logic
    }
}
