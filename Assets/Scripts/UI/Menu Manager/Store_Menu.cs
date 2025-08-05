using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Store_Menu : Base_Menu
{
    [SerializeField] private Button backButton;
    [SerializeField] private Button equipButton;

    [SerializeField] private Transform characterButtonContainer;
    [SerializeField] private Character_Button characterButtonPrefab;
    [SerializeField] private TMP_Text characterNameText;
    [SerializeField] private Image characterImage;
    [SerializeField] private TMP_Text totalStarsText;

    [SerializeField] private string fileName = "store.json";

    private List<Character_Data> _charactersList;
    private List<Character_Button> _activeCharacterButtons = new List<Character_Button>();

    public static int _selectedCharacterIndex = 0;

    private async void Awake()
    {
        Character_Button.OnCharacterSelected += DisplayCharacterDetails;
        backButton.onClick.AddListener(() => Menu_Manager.Instance.SwitchMenu(MenuState.Title));

        try
        {
            try
            {
                await UnityServices.InitializeAsync();
                AnalyticsService.Instance.StartDataCollection();
            }
            catch (ServicesInitializationException e)
            {
#if UNITY_EDITOR
                Debug.LogError($"Failed to initialize Unity Services: {e}");
#endif
            }

            equipButton.onClick.AddListener(() =>
            {
                Game_Manager.ActiveProfile.character = characterNameText.text;
                Analytics_Logger.Log(EventName.itemEquipped, (EventParameter.itemName, characterNameText.text));

                SwitchingCharacterModelCheck();
            });
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.LogError($"Failed to initialize Analytics: {e}");
#endif
        }
    }

    private void SwitchingCharacterModelCheck()
    {
        if (_selectedCharacterIndex < 0 || _selectedCharacterIndex >= _activeCharacterButtons.Count)
        {
#if UNITY_EDITOR
            Debug.LogWarning("Selected character index is out of bounds.");
#endif
            return;
        }

        if (_activeCharacterButtons[_selectedCharacterIndex].IsUnlocked)
        {
            var selectedCharacter = _activeCharacterButtons[_selectedCharacterIndex];
            var characterName = selectedCharacter.CharacterData.itemName;
            Model_Changer.ChangeModelAndSave?.Invoke(characterName);
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogWarning("Selected character is locked.");
#endif
        }
    }

    public override void Initialize()
    {
        LoadCharacterData();
        InitializeCharacterButtons();
        DisplayCharacterDetails(Game_Manager.ActiveProfile.character,
            Character_Button.LoadSpriteFromPath(_charactersList
                .Find(c => c.itemName == Game_Manager.ActiveProfile.character).imagePath));
    }

    private void LoadCharacterData()
    {
        // Paths for reading original JSON and copying it to persistentDataPath
        var streamingAssetsPath = Path.Combine(Application.streamingAssetsPath, fileName);
        var persistentPath = Path.Combine(Application.persistentDataPath, fileName);

        // Check if the file exists in persistentDataPath, if not, copy it from StreamingAssets
        if (!File.Exists(persistentPath))
        {
#if UNITY_EDITOR
            Debug.Log("Copying store data to persistentDataPath...");
#endif
            if (Application.platform == RuntimePlatform.Android)
            {
                // Use UnityWebRequest for Android due to APK compression
                StartCoroutine(CopyFileFromStreamingAssets(streamingAssetsPath, persistentPath));
            }
            else
            {
                File.Copy(streamingAssetsPath, persistentPath);
            }
        }

        // Load the JSON file from persistentDataPath
        _charactersList = JsonHelper.LoadList<Character_Data>(persistentPath);
    }

    private IEnumerator CopyFileFromStreamingAssets(string sourcePath, string destinationPath)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(sourcePath))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                File.WriteAllBytes(destinationPath, request.downloadHandler.data);
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogError($"Failed to copy file from StreamingAssets: {request.error}");
#endif
            }
        }
    }


    private void InitializeCharacterButtons()
    {
        // Clear existing buttons
        if (Game_Manager.ActiveProfile == null) return;

        foreach (Transform child in characterButtonContainer)
        {
            Destroy(child.gameObject);
        }

        // Get the total stars from their profile
        var totalStars = Game_Manager.ActiveProfile.totalStarsEarned;
        totalStarsText.text = totalStars.ToString();

        var i = 0;
        foreach (var character in _charactersList)
        {
            // Instantiate button prefab and initialize it
            var newCharacterButton = Instantiate(characterButtonPrefab, characterButtonContainer);
            newCharacterButton.InitializeCharacterButton(character, totalStars, i);
            _activeCharacterButtons.Add(newCharacterButton);
            newCharacterButton.AddListener();
            i++;
        }
    }


    private void DisplayCharacterDetails(string characterName, Sprite sprite)
    {
        characterNameText.text = characterName;
        characterImage.sprite = sprite;
        if (characterName == "Locked")
        {
            characterImage.color = Color.black;
            equipButton.interactable = false;
        }
        else
        {
            characterImage.color = Color.white;
            equipButton.interactable = true;
        }
    }
}

[Serializable]
public class Character_Data
{
    public string itemName;
    public string imagePath;
    public int starsToUnlock;
}