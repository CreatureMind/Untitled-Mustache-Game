using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Character_Button : MonoBehaviour
{
    [SerializeField] private Button characterButton;
    [SerializeField] private Image characterImage;
    [SerializeField] private TMP_Text starsText;
    
    private Character_Data _characterData;
    private bool _isUnlocked;

    public static Action<string, Sprite> OnCharacterSelected { get; set; }

    public void InitializeCharacterButton(Character_Data data, int totalStars)
    {
        _characterData = data;

        // Load and set the character's image
        var sprite = LoadSpriteFromPath(_characterData.imagePath);
        characterImage.sprite = sprite;
        
        starsText.text = _characterData.starsToUnlock.ToString();

        // Determine if character is unlocked
        _isUnlocked = totalStars >= _characterData.starsToUnlock;
        
        // Apply black and white color if locked
        characterImage.color = !_isUnlocked ? Color.black : Color.white;

        // Add button click listener
        characterButton.onClick.AddListener(OnCharacterButtonClicked);
    }

    private void OnCharacterButtonClicked()
    {
        OnCharacterSelected?.Invoke(_isUnlocked ? _characterData.itemName : "Locked", characterImage.sprite);
    }

    public static Sprite LoadSpriteFromPath(string path)
    {
        // Assuming Resources.Load is used for loading Sprite
        return Resources.Load<Sprite>(path);
    }
}
