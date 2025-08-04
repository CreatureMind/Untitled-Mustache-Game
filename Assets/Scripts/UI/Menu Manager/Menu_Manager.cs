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

    public void InitializeAllMenus()
    {
        foreach (var menu in _menus.Values)
        {
            menu.Initialize();
        }
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
#if UNITY_EDITOR

                Debug.LogWarning($"Menu with state {menu.MenuState} already exists. Skipping addition.");
#endif
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
    EndPopUp,
    InGame,
    Profile
}