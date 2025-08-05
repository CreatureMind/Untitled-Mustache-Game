using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Daily_Reward_Menu : Base_Menu
{
    [SerializeField] private List<Daily_Button> dailyRewards;
    [SerializeField] private Button backButton;
    [SerializeField] private string storeFilePath = "store.json";

    private Profile_Data ActiveProfile;
    
    private void Awake()
    {
        dailyRewards = new List<Daily_Button>(GetComponentsInChildren<Daily_Button>());
        backButton.onClick.AddListener(() => Menu_Manager.Instance.SwitchMenu(MenuState.Title));
        if (dailyRewards.Count == 0)
        {
#if UNITY_EDITOR
            Debug.LogError("No Daily_Button components found in the scene!");
#endif
        }
    }

    public override void Initialize()
    {
        ActiveProfile = Game_Manager.ActiveProfile;
        UpdateDailyRewards();
    }

    private void UpdateDailyRewards()
    {
        if (ActiveProfile == null)
        {
#if UNITY_EDITOR
            Debug.LogError("Active profile is null! Cannot update daily rewards.");
#endif
            return;
        }

        var streak = ActiveProfile.dailyStreak;
        var lastRewardDate = ActiveProfile.lastRewardDate;
        var currentDate = DateTime.Now.ToString("yyyy-MM-dd");

        // Reset the streak if the player skipped a day
        if (!string.IsNullOrEmpty(lastRewardDate) && lastRewardDate != currentDate)
        {
            var lastRewardDateTime = DateTime.Parse(lastRewardDate);
            if ((DateTime.Now - lastRewardDateTime).Days > 1)
            {
                streak = 0; // Reset streak if a day was skipped
            }
        }

        // Check if the player's streak exceeded the 7th day and reset it
        if (streak >= 7)
        {
            streak = 0; // Reset streak after the 7th day
        }

        // Update the buttons based on the streak
        for (var i = 0; i < dailyRewards.Count; i++)
        {
            var rewardButton = dailyRewards[i];
            var isCurrentDay = i == streak;

            if (rewardButton == null)
            {
#if UNITY_EDITOR
                Debug.LogError($"Button at index {i} in `dailyRewards` array is null.");
#endif
                continue;
            }

            // Check for UI components within the button
            var lockImage = rewardButton.GetComponent<Daily_Button>().LockImage;
            var checkmarkImage = rewardButton.GetComponent<Daily_Button>().CheckmarkImage;

            // Enable the current day's reward
            if (isCurrentDay)
            {
                rewardButton.Button.interactable = true;
                rewardButton.GetComponent<Image>().color = Color.white;
                lockImage.gameObject.SetActive(false);
                checkmarkImage.gameObject.SetActive(false);

                rewardButton.Button.onClick.RemoveAllListeners();
                rewardButton.Button.onClick.AddListener(() => ClaimReward(streak));
            }
            else
            {
                rewardButton.Button.interactable = false;

                // Set past days' colors and show the "V" image if applicable
                if (i < streak)
                {
                    rewardButton.GetComponent<Image>().color = Color.grey;
                    rewardButton.Button.interactable = false;
                    checkmarkImage.gameObject.SetActive(true);
                    lockImage.gameObject.SetActive(false);
                }
                else
                {
                    rewardButton.GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f, 1f);
                    rewardButton.Button.interactable = false;
                    checkmarkImage.gameObject.SetActive(false);
                    lockImage.gameObject.SetActive(true);
                }
            }
        }

        PlayerPrefs.SetString("LastProfile", ActiveProfile.nickname); // Redundancy for profile persistence
        PlayerPrefs.Save();
    }

    private void ClaimReward(int dayIndex)
    {
        if (ActiveProfile == null)
        {
#if UNITY_EDITOR
            Debug.LogError("Active profile is null! Cannot claim reward.");
#endif
            return;
        }

        // Add stars based on the day (from the enum)
        var rewardStars = (int)(DailyReward)Enum.Parse(typeof(DailyReward), $"Day{dayIndex + 1}");
        ActiveProfile.totalStarsEarned += rewardStars;

        // If it's the 7th day, add the special item to the store
        if (dayIndex == 6)
        {
            AddNewItemToStore();
        }

        // Update profile details
        ActiveProfile.dailyStreak++;
        ActiveProfile.lastRewardDate = DateTime.Now.ToString("yyyy-MM-dd");

        Game_Manager.Instance.SaveProfile(ActiveProfile); // Save profile progress

        Menu_Manager.Instance.SwitchMenu(MenuState.Title); // Hide the menu

        Menu_Manager.Instance.InitializeAllMenus();

        UpdateDailyRewards(); // Update UI
    }

    private void AddNewItemToStore()
    {
        var filePath = Path.Combine(Application.streamingAssetsPath, storeFilePath);

        if (!File.Exists(filePath))
        {
#if UNITY_EDITOR
            Debug.LogError($"Store file not found at {filePath}");
#endif
            return;
        }

        // Load existing store data
        var storeItems = JsonHelper.LoadList<Character_Data>(filePath);

        // Create a new store item
        var newItem = new Character_Data
        {
            itemName = "Tree",
            imagePath = "2DAssets/Special/Tree",
            starsToUnlock = 24
        };

        // Add, save, and update the store file
        storeItems.Add(newItem);
        JsonHelper.SaveList(filePath, storeItems);
    }
}

public enum DailyReward
{
    Day1 = 1,
    Day2 = 1,
    Day3 = 2,
    Day4 = 2,
    Day5 = 3,
    Day6 = 3,
    Day7 = 0,
}