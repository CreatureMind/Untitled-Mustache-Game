
using System;
using UnityEngine;
using UnityEngine.UI;

public class Settings_Screen: Base_Menu
{
    [SerializeField] private Button musicToggleButton;
    [SerializeField] private Button sfxToggleButton;
    [SerializeField] private Button darkModeToggleButton;
    [SerializeField] private Button unlockAllButton;
    [SerializeField] private Button resetGameDataButton;
    
    [SerializeField] private Button aboutButton;
    [SerializeField] private Button backButton;

    private void Awake()
    {
        aboutButton.onClick.AddListener(() => Menu_Manager.Instance.SwitchMenu(MenuState.About));
        backButton.onClick.AddListener(() => Menu_Manager.Instance.SwitchMenu(MenuState.Title));
    }
}
