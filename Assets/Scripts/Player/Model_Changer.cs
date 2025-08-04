using System;
using System.Collections.Generic;
using UnityEngine;

public class Model_Changer : MonoBehaviour
{
    [SerializeField] private List<GameObject> characterModels;
    private Dictionary<string, GameObject> modelDictionary;
    private string currentModel;
    
    private const string DefaultCharacterName = "DefaultCharacter"; // Default character name

    public static Action<string> ChangeModel;
    
    private void Awake()
    {
        ChangeModel += EquipCharacterModel;
        
        CreateDictionaryPool();
        
    }

    private void CreateDictionaryPool()
    {
        modelDictionary = new Dictionary<string, GameObject>();
        foreach (var model in characterModels)
        {
            var obj = Instantiate(model, transform);
            modelDictionary.Add(model.name, obj);
        }
    }

    private void EquipCharacterModel(string modelKey)
    {
        if (modelDictionary.TryGetValue(modelKey, out var model))
        {
            foreach (var kvp in modelDictionary)
            {
                kvp.Value.SetActive(false); // Deactivate all models
            }
            model.SetActive(true); // Activate the selected model
        }
        else
        {
            Debug.LogWarning($"Character model for '{modelKey}' not found.");
        }
    }

    
    private void OnDestroy()
    {
        ChangeModel -= EquipCharacterModel; // Unsubscribe from the event
    }
}
