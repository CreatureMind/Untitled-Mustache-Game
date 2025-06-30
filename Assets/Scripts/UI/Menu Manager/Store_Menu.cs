using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;

public class Store_Menu : Base_Menu
{
    [SerializeField] private Button backButton;
    [SerializeField] private Button buyButton;
    

    private async void  Awake()
    {
        AnalyticsService.Instance.StartDataCollection();
        await UnityServices.InitializeAsync();
        backButton.onClick.AddListener(() => Menu_Manager.Instance.SwitchMenu(MenuState.Title));
        buyButton.onClick.AddListener(() => 
        {
            CustomEvent e = new CustomEvent("boughtItem") { { "item", "Ogre Face" } };
            AnalyticsService.Instance.RecordEvent(e);
            AnalyticsService.Instance.Flush();
            Debug.Log("Buy button clicked");
        });
    }
    
}
