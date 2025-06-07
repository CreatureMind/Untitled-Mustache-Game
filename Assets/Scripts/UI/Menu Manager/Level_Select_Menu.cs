
using System;
using UnityEngine;
using UnityEngine.UI;

public class Level_Select_Menu : Base_Menu
{
    [SerializeField] private Image emptyStar;
    [SerializeField] private Image filledStar;
    [SerializeField] private Button backButton;

    private void Awake()
    {
        backButton.onClick.AddListener(() => Menu_Manager.Instance.SwitchMenu(MenuState.Title));
    }
}

[Serializable]
public class Level_Button
{
    private Button levelButton;
    private Button normalDifficultyButton;
    private Button infiniteDifficultyButton;

    private bool isNormal;
    
    private bool isLocked;
    private bool isInfiniteLocked;
    
    private Image mapImage;
    
    private Image star1;
    private Image star2;
    private Image star3;
    //Game_Manager.Instance.StartLevel(levelIndex, difficulty);
    
}
