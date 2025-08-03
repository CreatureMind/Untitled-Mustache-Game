using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Daily_Reward_Menu : MonoBehaviour
{
    [SerializeField] private List<Button> dailyRewards;
    [SerializeField] private string storeFilePath = "store.json";
    
    private Profile_Data ActiveProfile => Profile_Menu.ActiveProfile;

    private void Start()
    {
        UpdateDailyRewards();
    }

    private void UpdateDailyRewards()
    {
        if (ActiveProfile == null)
        {
            Debug.LogError("Active profile is null! Cannot update daily rewards.");
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
                Debug.LogError($"Button at index {i} in `dailyRewards` array is null.");
                continue;
            }

            // Check for UI components within the button
            var lockImage = rewardButton.GetComponent<Daily_Button>().lockImage;
            var checkmarkImage = rewardButton.GetComponent<Daily_Button>().checkmarkImage;

            // Enable the current day's reward
            if (isCurrentDay)
            {
                rewardButton.interactable = true;
                rewardButton.GetComponent<Image>().color = Color.white;
                lockImage.gameObject.SetActive(false);
                checkmarkImage.gameObject.SetActive(false);

                rewardButton.onClick.RemoveAllListeners();
                rewardButton.onClick.AddListener(() => ClaimReward(streak));
            }
            else
            {
                rewardButton.interactable = false;

                // Set past days' colors and show the "V" image if applicable
                if (i < streak)
                {
                    rewardButton.GetComponent<Image>().color = Color.grey; // Mark as claimed
                    if (checkmarkImage) checkmarkImage.gameObject.SetActive(true); // Show "V" sprite
                }
                else
                {
                    rewardButton.GetComponent<Image>().color = Color.black; // Inactive for future days
                    rewardButton.interactable = false; // Disable button for future days
                }

                if (lockImage) lockImage.gameObject.SetActive(true); // Show "Lock" sprite
            }
        }

        PlayerPrefs.SetString("LastProfile", ActiveProfile.nickname); // Redundancy for profile persistence
        PlayerPrefs.Save();
    }
    
    private void ClaimReward(int dayIndex)
    {
        if (ActiveProfile == null)
        {
            Debug.LogError("Active profile is null! Cannot claim reward.");
            return;
        }

        // Add stars based on the day (from the enum)
        var rewardStars = (int)(DailyReward)Enum.Parse(typeof(DailyReward), $"Day{dayIndex + 1}");
        ActiveProfile.totalStarsEarned += rewardStars;

        // If the 7th day, add a new item to the store
        if (dayIndex == 6)
        {
            AddNewItemToStore();
        }

        // Update profile details
        ActiveProfile.dailyStreak++;
        ActiveProfile.lastRewardDate = DateTime.Now.ToString("yyyy-MM-dd");

        Game_Manager.Instance.SaveProgress(); // Save profile progress

        // Update UI
        UpdateDailyRewards();
    }

    private void AddNewItemToStore()
    {
        var filePath = Path.Combine(Application.streamingAssetsPath, storeFilePath);

        if (!File.Exists(filePath))
        {
            Debug.LogError($"Store file not found at {filePath}");
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
        JsonHelper.SaveList(filePath, storeItems);;
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