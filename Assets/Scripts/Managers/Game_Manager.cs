using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Game_Manager : MonoBehaviour
{
    public static Game_Manager Instance { get; private set; }
    
    public static Profile_Data ActiveProfile { get; set; }
    
    //should be data list
    
    private List<Progress_Data> _progress;
    public List<Progress_Data> Progress
    {
        get
        {
            if (ActiveProfile == null || string.IsNullOrEmpty(ActiveProfile.progressPath))
                return new List<Progress_Data>(); // Return empty list if no profile or progress path

            var progress = JsonHelper.LoadList<Progress_Data>(ActiveProfile.progressPath);
            return progress ?? new List<Progress_Data>(); // Return empty list if loading fails
        }
    }
    
    private Settings_Data _settings;
    public Settings_Data Settings 
    {
        get
        {
            if (ActiveProfile == null || string.IsNullOrEmpty(ActiveProfile.settingsPath))
                return new Settings_Data(); // Return default settings if no profile or settings path

            var settings = JsonHelper.Load<Settings_Data>(ActiveProfile.settingsPath);
            return settings ?? new Settings_Data(); // Return default if loading fails
        }
    }

    private void Awake()
    {
        Level_Manager.RoundScoreCalculated += EditProgressToSave;
        Model_Changer.ChangeModelAndSave += CharacterChanged;
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

        if (!string.IsNullOrEmpty(nickname) && Directory.Exists(Profile_Menu.ProfilesPath))
        {
            var directories = Directory.GetDirectories(Profile_Menu.ProfilesPath);
            nickname = directories.FirstOrDefault(dir => Path.GetFileName(dir) == nickname) != null 
                ? nickname 
                : Path.GetFileName(directories.FirstOrDefault());
        }
        else
        {
            Profile_Menu.CurrentProfileState = Profile_State.New;
            Menu_Manager.Instance.SwitchMenu(MenuState.Profile);
        }
        
        var profile = Profile_Menu.LoadProfileByNickname(nickname);
        if (profile == null) return;

        ActiveProfile = profile;
        _progress = JsonHelper.LoadList<Progress_Data>(profile.progressPath);
        _settings = JsonHelper.Load<Settings_Data>(profile.settingsPath);
        
        Debug.Log(ActiveProfile);
    }
    
    private void EditProgressToSave(int index, int starsEarned)
    {
        if (ActiveProfile == null || string.IsNullOrEmpty(ActiveProfile.progressPath)) return;

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
        
        if (ActiveProfile != null)
        {
            ActiveProfile.totalStarsEarned = 0;
            foreach (var progress in _progress)
            {
                ActiveProfile.totalStarsEarned += progress.starsEarned;
            }
            SaveProfile(ActiveProfile);
        }
    }
    
    public void CharacterChanged(string modelName)
    {
        if (ActiveProfile == null) return;

        ActiveProfile.character = modelName;
        SaveProfile(ActiveProfile);
    }

    public void SaveProfile(Profile_Data profile)
    {
        if (profile == null || string.IsNullOrEmpty(profile.nickname)) return;

        var profilePath = Path.Combine(Profile_Menu.ProfilesPath, profile.nickname, "profile.json");
        JsonHelper.Save(profilePath, profile);
#if UNITY_EDITOR
        Debug.Log($"Profile saved to: {profilePath}");
#endif
        
        PlayerPrefs.SetString("LastProfile", profile.nickname);
        PlayerPrefs.Save();
    }

    public void SaveProgress(List<Progress_Data> progress)
    {
        if (ActiveProfile == null || string.IsNullOrEmpty(ActiveProfile.progressPath)) return;
        _progress = progress;
        JsonHelper.SaveList(ActiveProfile.progressPath, _progress);
    }

    public void SaveSettings(Settings_Data settingsData)
    {
        if (ActiveProfile == null || string.IsNullOrEmpty(ActiveProfile.settingsPath)) return;
        _settings = settingsData;
        JsonHelper.Save(ActiveProfile.settingsPath, _settings);
    }

    public void SwitchProfile(Profile_Data newProfile)
    {
        ActiveProfile = newProfile;
        _progress = JsonHelper.LoadList<Progress_Data>(ActiveProfile.progressPath);
        _settings = JsonHelper.Load<Settings_Data>(ActiveProfile.settingsPath);
        PlayerPrefs.SetString("LastProfile", newProfile.nickname);
        PlayerPrefs.Save();
        
        Debug.Log(ActiveProfile);
        //Model_Changer.ChangeModelFromLoad?.Invoke();
    }
}
