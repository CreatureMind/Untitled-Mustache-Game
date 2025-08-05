using System;
using System.Collections.Generic;
using UnityEngine;

public class Model_Changer : MonoBehaviour
{
    [SerializeField] private List<GameObject> characterModels;
    private Dictionary<string, GameObject> ModelDictionary { get; set; }
    private string currentModel;

    private const string DefaultCharacterName = "DefaultCharacter"; // Default character name

    public static Action<string> ChangeModelAndSave;
    public static Action ChangeModelFromLoad;

    private void Awake()
    {
        ChangeModelAndSave += EquipCharacterModel;
        ChangeModelFromLoad += LoadLastUsedModel;

        CreateDictionaryPool();
        
        LoadLastUsedModel();
    }
    
    private void LoadLastUsedModel()
    {
        if (Game_Manager.ActiveProfile != null && !string.IsNullOrEmpty(Game_Manager.ActiveProfile.character))
        {
            currentModel = Game_Manager.ActiveProfile.character;
            EquipCharacterModel(currentModel);
        }
        else
        {
            // If no profile or character is set, equip the default character
            EquipCharacterModel(DefaultCharacterName);
        }
    }

    private void CreateDictionaryPool()
    {
        ModelDictionary = new Dictionary<string, GameObject>();
        foreach (var model in characterModels)
        {
            var obj = Instantiate(model, transform);
            ModelDictionary.Add(model.name, obj);
        }
    }

    private void EquipCharacterModel(string modelKey)
    {
        if (ModelDictionary.TryGetValue(modelKey, out var model))
        {
            foreach (var kvp in ModelDictionary)
            {
                kvp.Value.SetActive(false); // Deactivate all models
            }

            model.SetActive(true); // Activate the selected model
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogWarning($"Character model for '{modelKey}' not found.");
#endif
        }
    }


    private void OnDestroy()
    {
        ChangeModelAndSave -= EquipCharacterModel; 
        ChangeModelFromLoad -= LoadLastUsedModel;
    }
}