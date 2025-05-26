using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;
using TMPro;

public class ButtonLoader : MonoBehaviour
{
    [Header("References")]
    public GameObject buttonPrefab;
    public Transform parentContainer;

    [Header("JSON File")]
    public string fileName = "buttons.json";

    private void Start()
    {
        string path = Path.Combine(Application.streamingAssetsPath, fileName);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            List<ButtonData> buttonDataList = JsonHelper.FromJson<ButtonData>(json);

            foreach (var data in buttonDataList)
            {
                GameObject newButton = Instantiate(buttonPrefab, parentContainer);
                newButton.name = data.name;

                // Set button text
                TextMeshProUGUI tmpText = newButton.GetComponentInChildren<TextMeshProUGUI>();
                if (tmpText != null)
                    tmpText.text = data.name;

                // Set button color
                Color parsedColor;
                if (ColorUtility.TryParseHtmlString(data.color, out parsedColor))
                {
                    Image image = newButton.GetComponent<Image>();
                    if (image != null)
                        image.color = parsedColor;
                }

                // Assign scene loading behavior
                Button btn = newButton.GetComponent<Button>();
                int sceneIndex = data.sceneIndex;
                btn.onClick.AddListener(() => SceneManager.LoadScene(sceneIndex));
            }
        }
        else
        {
            Debug.LogError($"JSON file not found at path: {path}");
        }
    }
}

[System.Serializable]
public class ButtonData
{
    public string name;
    public string color;
    public int sceneIndex;
}