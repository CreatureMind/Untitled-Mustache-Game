
using UnityEngine;
using UnityEngine.UI;

public class Title_Menu: Base_Menu
{
    [SerializeField] private Button LevelSelectButton;
    [SerializeField] private Button storeButton;
    [SerializeField] private Button settingsButton;

    private void Awake()
    {
        LevelSelectButton.onClick.AddListener(() => Menu_Manager.Instance.SwitchMenu(MenuState.LevelSelect));
        storeButton.onClick.AddListener(() => Menu_Manager.Instance.SwitchMenu(MenuState.Store));
        settingsButton.onClick.AddListener(() => Menu_Manager.Instance.SwitchMenu(MenuState.Settings));
    }

    protected override void OnMenuOpen()
    {
    }
}
