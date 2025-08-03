using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Level_Select_Menu : Base_Menu
{
    [SerializeField] private Button backButton;

    [SerializeField] private Transform levelButtonContainer;
    [SerializeField] private Level_Button levelButtonPrefabScript;
    [SerializeField] private string fileName = "Level_Button_Data.json";
    
    private List<Level_Button_Data> _levelButtonDataList;

    private void Awake()
    {
        backButton.onClick.AddListener(() => Menu_Manager.Instance.SwitchMenu(MenuState.Title));
    }
    
    private void Start()
    {
        LoadLevelButtonData();
        InitializeLevelButtons();
    }

    private void LoadLevelButtonData()
    {
        var path = Path.Combine(Application.streamingAssetsPath, fileName);
        _levelButtonDataList = JsonHelper.LoadList<Level_Button_Data>(path);
    }
    
    private void InitializeLevelButtons()
    {
        foreach (var buttonData in _levelButtonDataList)
        {
            var newButton = Instantiate(levelButtonPrefabScript, levelButtonContainer);
            newButton.InitalizeLevelButton(buttonData);
        }
    }

    public static void LevelButtonClicked(Level_Button levelButton)
    {
        var levelIndex = levelButton.LevelButtonData.levelIndex;
        Level_Manager.Instance.StartLevel(levelIndex,
            levelButton.IsNormalDifficultySelected ? Difficulty.Normal : Difficulty.Infinite);

        Menu_Manager.Instance.SwitchMenu(MenuState.InGame);
    }
}

[Serializable]
public class Level_Button_Data
{
    public int levelIndex;
    public string levelName;
    public string pathToMapImage;
    public int starsEarned;
    public LevelStateType levelStateType;

    public Level_Button_Data(string levelName, int levelIndex, string pathToMapImage)
    {
        this.levelName = levelName;
        this.levelIndex = levelIndex;
        this.pathToMapImage = pathToMapImage;

        this.starsEarned = Game_Manager.Instance.Progress[levelIndex].starsEarned;

        switch (starsEarned)
        {
            case 0:
                if (levelIndex == 0)
                {
                    levelStateType = LevelStateType.Normal;
                }

                int prevLevelStarsEarned = Game_Manager.Instance.Progress[levelIndex - 1].starsEarned;
                if (prevLevelStarsEarned > 0)
                    levelStateType = LevelStateType.Normal;
                else
                    levelStateType = LevelStateType.Locked;
                break;

            case 1:
            case 2:
                levelStateType = LevelStateType.Normal;
                break;
            case 3:
                levelStateType = LevelStateType.Infinite;
                break;
            default:
                levelStateType = LevelStateType.Locked;
                break;
        }
    }
}

public enum LevelStateType
{
    Locked = 0,
    Normal = 1,
    Infinite = 2
}