using System;
using UnityEngine;
using Unity.Services.Analytics;

public enum EventName
{
    itemBought,
    itemEquipped,
}

public enum EventParameter
{
    itemName,
    itemID,
}

public static class Analytics_Logger
{
    /// <summary>
    /// Sends a custom event with standard metadata automatically included.
    /// </summary>
    /// <param name="eventName">The name of the custom event (must match schema if validation is enabled).</param>
    /// <param name="customData">Custom key-value pairs specific to the event.</param>
    public static void Log(EventName eventName, params (EventParameter key, object value)[] customData)
    {
        var e = new CustomEvent(eventName.ToString())
        {
            // Add standard metadata
            { "timestamp", DateTime.UtcNow.ToString("o") },
            { "clientVersion", Application.version },
            { "platform", Application.platform.ToString() },
            { "sdkMethod", "AnalyticsLogger" }
        };

        // Add custom data
        foreach (var (key, value) in customData)
        {
            e.Add(key.ToString(), value);
        }

        AnalyticsService.Instance.RecordEvent(e);
#if UNITY_EDITOR
        AnalyticsService.Instance.Flush(); // Optional immediate upload in the editor
#endif
    }
}