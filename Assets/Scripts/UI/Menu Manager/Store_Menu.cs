using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
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
                Debug.LogError($"Failed to initialize Unity Services: {e}");
            }

            equipButton.onClick.AddListener(() =>
            {
                Profile_Menu.ActiveProfile.character = characterNameText.text;
                Analytics_Logger.Log(EventName.itemEquipped, (EventParameter.itemName, "Ogre Face"));
                Debug.Log("Buy button clicked");

                SwitchingCharacterModelCheck();
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to initialize Analytics: {e}");
        }
    }

    private void SwitchingCharacterModelCheck()
    {
        if (_selectedCharacterIndex < 0 || _selectedCharacterIndex >= _activeCharacterButtons.Count)
        {
            Debug.LogWarning("Selected character index is out of bounds.");
            return;
        }

        if (_activeCharacterButtons[_selectedCharacterIndex].IsUnlocked)
        {
            var selectedCharacter = _activeCharacterButtons[_selectedCharacterIndex];
            string characterName = selectedCharacter.CharacterData.itemName;
            Model_Changer.ChangeModel?.Invoke(characterName);
        }
        else
        {
            Debug.LogWarning("Selected character is locked.");
        }
    }

    public override void Initialize()
    {
        LoadCharacterData();
        InitializeCharacterButtons();
        DisplayCharacterDetails(Profile_Menu.ActiveProfile.character,
            Character_Button.LoadSpriteFromPath(_charactersList
                .Find(c => c.itemName == Profile_Menu.ActiveProfile.character).imagePath));
    }

    private void LoadCharacterData()
    {
        var path = Path.Combine(Application.streamingAssetsPath, fileName);
        _charactersList = JsonHelper.LoadList<Character_Data>(path);
    }

    private void InitializeCharacterButtons()
    {
        // Clear existing buttons
        if (Profile_Menu.ActiveProfile != null)
        {
            foreach (Transform child in characterButtonContainer)
            {
                Destroy(child.gameObject);
            }
        }

        // Get player's total stars from their profile
        var totalStars = Profile_Menu.ActiveProfile.totalStarsEarned;
        totalStarsText.text = totalStars.ToString();

        int i = 0;
        foreach (var character in _charactersList)
        {
            // Instantiate button prefab and initialize it
            Character_Button newCharacterButton = Instantiate(characterButtonPrefab, characterButtonContainer);
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