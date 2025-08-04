using System;
using UnityEngine;

public class End_Screen : Base_Menu
{
    [SerializeField] private Win_Menu winPopPanel;
    [SerializeField] private Lose_Menu losePopPanel;

    private void Awake()
    {
        Level_Manager.RoundScoreCalculated += HandleEndPopUpLogic;
    }

    private void HandleEndPopUpLogic(int index, int stars)
    {
        Menu_Manager.Instance.SwitchMenu(MenuState.EndPopUp);
        if (stars == 0)
        {
            losePopPanel.Show();
            winPopPanel.Hide();
        }
        else
        {
            winPopPanel.Show();
            winPopPanel.SetStars(stars);
            losePopPanel.Hide();
        }
    }

    protected override void OnMenuClose()
    {
        winPopPanel.Hide();
        losePopPanel.Hide();
    }
}