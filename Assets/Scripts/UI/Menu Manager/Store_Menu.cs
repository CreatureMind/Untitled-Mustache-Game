using System;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;

public class Store_Menu : Base_Menu
{
    [SerializeField] private Button backButton;
    [SerializeField] private Button buyButton;


    private async void Awake()
    {
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

            buyButton.onClick.AddListener(() =>
            {
                Analytics_Logger.Log(EventName.itemEquipped,(EventParameter.itemName,"Ogre Face"));
                Debug.Log("Buy button clicked");
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to initialize Analytics: {e}");
        }
    }
}