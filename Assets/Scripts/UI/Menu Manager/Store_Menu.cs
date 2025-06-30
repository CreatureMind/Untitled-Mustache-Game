using UnityEngine;
using UnityEngine.UI;

public class Store_Menu : Base_Menu
{
    [SerializeField] private Button backButton;
    [SerializeField] private Button buyButton;

    private void Awake()
    {
        backButton.onClick.AddListener(() => Menu_Manager.Instance.SwitchMenu(MenuState.Title));
        buyButton.onClick.AddListener(() => 
        {
            // Implement buy functionality here
            Debug.Log("Buy button clicked");
        });
    }
}
