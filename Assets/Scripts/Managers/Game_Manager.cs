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
