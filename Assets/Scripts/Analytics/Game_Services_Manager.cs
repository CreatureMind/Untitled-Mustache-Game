using System;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Analytics;

public class Game_Services_Manager : MonoBehaviour
{
    public static Game_Services_Manager Instance { get; private set; }
    public bool IsInitialized { get; private set; } = false;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        _ = InitializeServices(); // fire and forget
    }

    async Task InitializeServices()
    {
        try
        {
            await UnityServices.InitializeAsync();

            // Analytics
            AnalyticsService.Instance.StartDataCollection();
        }
        catch (Exception e)
        {
            Debug.LogError($"[GameServicesManager] Initialization failed: {e}");
        }
    }
}