using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class JsonHelper
{
    // Deserialize list from JSON string
    public static List<T> FromJson<T>(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            Debug.LogError("Error: Attempting to deserialize an empty or null JSON string.");
            return new List<T>();
        }

        try
        {
            // Check if the JSON is a plain array (starts with '[')
            if (json.Trim().StartsWith("[") && json.Trim().EndsWith("]"))
            {
                // Wrap the array with "Items" to fit the Wrapper<T> structure
                json = "{\"Items\":" + json + "}";
            }

            // Deserialize using the wrapper
            var wrapper = JsonUtility.FromJson<Wrapper<T>>(json);

            // Null safety for Items
            if (wrapper == null || wrapper.Items == null)
            {
                Debug.LogWarning("Warning: No items were found in the JSON or deserialization failed.");
                return new List<T>();
            }

            return new List<T>(wrapper.Items);
        }
        catch (Exception ex)
        {
            // Log parsing errors
            Debug.LogError($"Error deserializing JSON: {ex.Message}");
            return new List<T>();
        }
    }

    // Serialize list to JSON string
    public static string ToJson<T>(List<T> list)
    {
        var wrapper = new Wrapper<T> { Items = list.ToArray() };
        return JsonUtility.ToJson(wrapper, true);
    }

    // Load single object from JSON file
    public static T Load<T>(string path) where T : new()
    {
        if (!File.Exists(path))
        {
            Debug.LogWarning($"JsonHelper.Load: File not found at {path}. Returning default {typeof(T).Name}.");
            return new T();
        }

        var json = File.ReadAllText(path);
        return JsonUtility.FromJson<T>(json);
    }

    // Save single object to JSON file
    public static void Save<T>(string path, T obj)
    {
        var json = JsonUtility.ToJson(obj, true);
        File.WriteAllText(path, json);
    }

    // Load list of objects from JSON file
    public static List<T> LoadList<T>(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("Error: The given file path is null or empty.");
            return new List<T>();
        }

        if (!File.Exists(path))
        {
            Debug.LogWarning($"Warning: File not found at path: {path}. Returning empty list.");
            return new List<T>();
        }

        try
        {
            var json = File.ReadAllText(path);
            return FromJson<T>(json); // Leverages fixed FromJson<T>
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error loading JSON data from path: {path}. Exception: {ex.Message}");
            return new List<T>();
        }
    }

    // Save list of objects to JSON file
    public static void SaveList<T>(string path, List<T> list)
    {
        var json = ToJson(list);
        File.WriteAllText(path, json);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}