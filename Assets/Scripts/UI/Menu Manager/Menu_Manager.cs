using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Menu_Manager : MonoBehaviour
{
    public static Menu_Manager Instance { get; private set; }

    private readonly Dictionary<MenuState, Base_Menu> _menus = new();
    [SerializeField] private List<Base_Menu> menuList;

    private Base_Menu _currentMenu;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        CreateAllMenus();

        SwitchMenu(Game_Manager.CheckFirstTime() ? MenuState.Profile : MenuState.Title);
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