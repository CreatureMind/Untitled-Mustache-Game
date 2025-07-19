using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Menu_Manager : MonoBehaviour
{
    private static Menu_Manager _instance;
    public static Menu_Manager Instance => _instance;

    private readonly Dictionary<MenuState, Base_Menu> _menus = new();
    [SerializeField] private List<Base_Menu> menuList;

    private Base_Menu _currentMenu;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;

        CreateAllMenus();

        SwitchMenu(Profile_Menu.IsFirstTime ? MenuState.Profile : MenuState.Title);
    }

    private void CreateAllMenus()
    {
        foreach (var menu in menuList)
        {
            if (_menus.TryAdd(menu.MenuState, menu))
            {
                menu.Hide();
            }
            else
            {
                Debug.LogWarning($"Menu with state {menu.MenuState} already exists. Skipping addition.");
            }
        }
    }

    public void SwitchMenu(MenuState state)
    {
        if (_currentMenu != null) _currentMenu.Hide();
        if (_menus.TryGetValue(state, out var menu))
        {
            _currentMenu = menu;
            _currentMenu.Show();
        }
    }
}

public enum MenuState
{
    Title,
    Store,
    LevelSelect,
    Settings,
    About,
    Win,
    Lose,
    InGame,
    Pause,
    Profile
}