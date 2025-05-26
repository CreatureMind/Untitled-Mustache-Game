using System;
using System.Collections.Generic;
using UnityEngine;

public static class JsonHelper
{
    public static List<T> FromJson<T>(string json)
    {
        string wrappedJson = "{\"Items\":" + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(wrappedJson);
        return new List<T>(wrapper.Items);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}