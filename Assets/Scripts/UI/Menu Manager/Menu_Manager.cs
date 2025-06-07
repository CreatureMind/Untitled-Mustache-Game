using System.Collections.Generic;
using UnityEngine;

    public class Menu_Manager : MonoBehaviour
    {
        private static Menu_Manager _instance;
        public static Menu_Manager Instance => _instance;

        private Dictionary<MenuState, Base_Menu> menus = new();
        [SerializeField] private List<Base_Menu> titleMenus;
        
        private Base_Menu currentMenu;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;

            CreateAllMenus();
        }

        private void CreateAllMenus()
        {
            foreach (Base_Menu menu in titleMenus)
            {
                if (!menus.ContainsKey(menu.MenuState))
                {
                    menus.Add(menu.MenuState, menu);
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
            if (currentMenu != null) currentMenu.Hide();
            if (menus.TryGetValue(state, out Base_Menu menu))
            {
                currentMenu = menu;
                currentMenu.Show();
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
}