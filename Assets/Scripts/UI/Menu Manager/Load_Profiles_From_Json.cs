using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Load_Profiles_From_Json : MonoBehaviour
{
    [SerializeField] private GameObject profileButton; // Reference to the button prefab
    [SerializeField] private Transform content; // Parent UI element (container for buttons)
    [SerializeField] private string fileName = "profile.json";

    private void Awake()
    {
        // Check if the ProfilesPath exists
        if (Directory.Exists(Profile_Menu.ProfilesPath))
        {
            // Get all directories inside the ProfilesPath
            var directories = Directory.GetDirectories(Profile_Menu.ProfilesPath);

            // Iterate through each folder
            foreach (var directory in directories)
            {
                var profilePath = Path.Join(directory, "/",fileName);
                Debug.Log($"Checking: {profilePath}");
                
                // Check if profile.json exists in the directory
                if (File.Exists(profilePath))
                {
                    try
                    {
                        // Read JSON content from the file
                        //var jsonContent = File.ReadAllText(profilePath);

                        // Parse the JSON into a Profile_Data object
                        var profileData = JsonHelper.Load<Profile_Data>(profilePath);

                        // Create a button for the profile
                        var buttonObj = Instantiate(profileButton, content);

                        // Configure the button (e.g., set its text to the profile name or folder name)
                        var button = buttonObj.GetComponent<Button>();
                        var buttonText = buttonObj.GetComponentInChildren<TMP_Text>();

                        // Use either the folder name or a property from the JSON as the button text
                        var folderName = Path.GetFileName(directory);
                        buttonText.text = string.IsNullOrEmpty(profileData.nickname) ? folderName : profileData.nickname;

                        // Assign button functionality
                        button.onClick.AddListener(() =>
                        {
                            // Set the active profile whenever this button is clicked
                            Game_Manager.Instance.SwitchProfile(profileData);
                            Debug.Log($"Active Profile set to: {profilePath}");
                            Debug.Log(profileData);
                            Menu_Manager.Instance.SwitchMenu(MenuState.Settings);
                        });
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError($"Failed to parse JSON in {profilePath}: {ex.Message}");
                    }
                }
                else
                {
                    Debug.LogWarning($"profile.json not found in: {directory}");
                }
            }
        }
        else
        {
            Debug.LogWarning("ProfilesPath does not exist.");
        }
    }
}