using System;
using System.Collections.Generic;
using UnityEngine;

public class Model_Changer : MonoBehaviour
{
    [SerializeField] private List<GameObject> characterModels;
    private Dictionary<string, GameObject> ModelDictionary { get; set; }
    private string currentModel;

    private const string DefaultCharacterName = "DefaultCharacter"; // Default character name

    public static Action<string> ChangeModel;

    private void Awake()
    {
        ChangeModel += EquipCharacterModel;

        CreateDictionaryPool();

        EquipCharacterModel(Profile_Menu.ActiveProfile.character);
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
        ChangeModel -= EquipCharacterModel; // Unsubscribe from the event
    }
}