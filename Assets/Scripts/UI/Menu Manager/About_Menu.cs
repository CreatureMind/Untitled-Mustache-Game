using System;
using UnityEngine;
using UnityEngine.UI;

public class About_Menu : Base_Menu
{
    [SerializeField] private Button backButton;

    private void Awake()
    {
        backButton.onClick.AddListener(() => Menu_Manager.Instance.SwitchMenu(MenuState.Settings));
    }
}