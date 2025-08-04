using System;
using UnityEngine;
using UnityEngine.UI;

public class Title_Menu: Base_Menu
{
    [SerializeField] private Button LevelSelectButton;
    [SerializeField] private Button storeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private GameObject dailyRewardPanel;
    
    private const string LastLoginKey = "LastLoginDate";

    private void Awake()
    {
        LevelSelectButton.onClick.AddListener(() => Menu_Manager.Instance.SwitchMenu(MenuState.LevelSelect));
        storeButton.onClick.AddListener(() => Menu_Manager.Instance.SwitchMenu(MenuState.Store));
        settingsButton.onClick.AddListener(() => Menu_Manager.Instance.SwitchMenu(MenuState.Settings));
    }

    protected override void OnMenuOpen()
    {
        Menu_Manager.Instance.InitializeAllMenus();
		Model_Changer.ChangeModel(Profile_Menu.ActiveProfile.character);
        
		// Check if the daily reward panel should be shown
        ShowDailyRewardPanelIfFirstLogin();
    }

    private void ShowDailyRewardPanelIfFirstLogin()
    {
        var activeProfile = Profile_Menu.ActiveProfile;
        if (activeProfile == null)
        {
            Debug.LogError("No active profile found! Ensure a profile is loaded before showing rewards.");
            dailyRewardPanel.SetActive(false);
            return;
        }

        var lastRewardDate = activeProfile.lastRewardDate;
        var currentDate = DateTime.Now.ToString("yyyy-MM-dd");

        // Show the daily reward panel if the current date is different from the last login date
        if (string.IsNullOrEmpty(lastRewardDate) || lastRewardDate != currentDate)
        {
            dailyRewardPanel.SetActive(true);
            activeProfile.lastRewardDate = currentDate;
            Game_Manager.Instance.SaveProfile(activeProfile);
        }
        else
        {
            dailyRewardPanel.SetActive(false);
        }
    }
}
