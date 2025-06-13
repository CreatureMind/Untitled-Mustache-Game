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
    
    [SerializeField] private string jsonPath = "Level_Button_Data.json";

    private void Awake()
    {
        backButton.onClick.AddListener(() => Menu_Manager.Instance.SwitchMenu(MenuState.Title));
        
        string path = Path.Combine(Application.streamingAssetsPath, jsonPath);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            List<Level_Button_Data> buttonDataList = JsonHelper.FromJson<Level_Button_Data>(json);

            foreach (Level_Button_Data buttonData in buttonDataList)
            {
                Debug.Log(buttonData.levelStateType);
                Level_Button newButton = Instantiate(levelButtonPrefabScript, levelButtonContainer);
                newButton.InitalizeLevelButton(buttonData);    
            }
        }
    }
    
    
}

[Serializable]
public class Level_Button_Data
{
    public string levelName;
    public string pathToMapImage;
    public int starsEarned;
    public LevelStateType levelStateType;
}
public enum LevelStateType
{
    Locked = 0,
    Normal = 1,
    Infinite = 2
}
