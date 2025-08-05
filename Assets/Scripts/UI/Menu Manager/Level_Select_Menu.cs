using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
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

    public override void Initialize()
    {
        LoadLevelButtonData();
    }

    protected override void OnMenuOpen()
    {
        InitializeLevelButtons();
    }

    private void LoadLevelButtonData()
    {
        // Paths for reading original JSON and copying it to persistentDataPath
        var streamingAssetsPath = Path.Combine(Application.streamingAssetsPath, fileName);
        var persistentPath = Path.Combine(Application.persistentDataPath, fileName);

        // Check if the file exists in persistentDataPath, if not, copy it from StreamingAssets
        if (!File.Exists(persistentPath))
        {
#if UNITY_EDITOR
            Debug.Log("Copying level button data to persistentDataPath...");
#endif
            if (Application.platform == RuntimePlatform.Android)
            {
                // Use UnityWebRequest for Android due to APK compression
                StartCoroutine(CopyFileFromStreamingAssets(streamingAssetsPath, persistentPath));
            }
            else
            {
                File.Copy(streamingAssetsPath, persistentPath);
            }
        }

        // Load the JSON file from persistentDataPath
        _levelButtonDataList = JsonHelper.LoadList<Level_Button_Data>(persistentPath);
    }

    private IEnumerator CopyFileFromStreamingAssets(string sourcePath, string destinationPath)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(sourcePath))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                File.WriteAllBytes(destinationPath, request.downloadHandler.data);
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogError($"Failed to copy file from StreamingAssets: {request.error}");
#endif
            }
        }
    }

    private void InitializeLevelButtons()
    {
        foreach (Transform child in levelButtonContainer)
        {
            Destroy(child.gameObject);
        }

        int totalProgressData = Game_Manager.Instance.Progress.Count;
        int starsEarned = 0;
        LevelStateType levelState;

        for (int i = 0; i < _levelButtonDataList.Count; i++)
        {
            var newButton = Instantiate(levelButtonPrefabScript, levelButtonContainer);
            if (i < totalProgressData)
            {
                starsEarned = Game_Manager.Instance.Progress[i].starsEarned;
                switch (starsEarned)
                {
                    case 0:
                        if (i == 0)
                        {
                            levelState = LevelStateType.Normal;
                        }
                        else
                        {
                            int prevLevelStarsEarned = Game_Manager.Instance.Progress[i - 1].starsEarned;
                            if (prevLevelStarsEarned > 0)
                                levelState = LevelStateType.Normal;
                            else
                                levelState = LevelStateType.Locked;
                        }

                        break;

                    case 1:
                    case 2:
                        levelState = LevelStateType.Normal;
                        break;
                    case 3:
                        levelState = LevelStateType.Infinite;
                        break;
                    default:
                        levelState = LevelStateType.Locked;
                        break;
                }
            }
            else if (i - 1 < totalProgressData && Game_Manager.Instance.Progress[i - 1].starsEarned > 0)
            {
                levelState = LevelStateType.Normal;
                starsEarned = 0;
            }
            else
            {
                starsEarned = 0;
                levelState = LevelStateType.Locked;
            }

            newButton.InitializeLevelButton(_levelButtonDataList[i], starsEarned, levelState);
        }
    }

    public static void LevelButtonClicked(Level_Button levelButton)
    {
        var levelIndex = levelButton.LevelButtonData.levelIndex;
        Level_Manager.Instance.StartLevel(levelIndex,
            levelButton.IsNormalDifficultySelected ? Difficulty.Normal : Difficulty.Infinite);
    }
}

[Serializable]
public class Level_Button_Data
{
    public int levelIndex;
    public string levelName;
    public string pathToMapImage;
}

public enum LevelStateType
{
    Locked = 0,
    Normal = 1,
    Infinite = 2
}