using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class JsonHelper
{
    // Deserialize list from JSON string
    public static List<T> FromJson<T>(string json)
    {
        var wrappedJson = "{\"Items\":" + json + "}";
        var wrapper = JsonUtility.FromJson<Wrapper<T>>(wrappedJson);
        return new List<T>(wrapper.Items);
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
        if (!File.Exists(path))
        {
            Debug.LogWarning($"JsonHelper.LoadList: File not found at {path}. Returning empty list.");
            return new List<T>();
        }

        var json = File.ReadAllText(path);
        return FromJson<T>(json);
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