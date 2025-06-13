using UnityEngine;
using UnityEngine.UI;
public class Level_Button : MonoBehaviour
{
    [SerializeField] private Button levelButton;
    [SerializeField] private Button normalDifficultyButton;
    [SerializeField] private Button infiniteDifficultyButton;
    [SerializeField] private TMPro.TextMeshProUGUI levelText;
    
    [SerializeField] private Image mapImage;
    
    [SerializeField] private Image[] stars;

    public void InitalizeLevelButton(Level_Button_Data levelButtonData)
    {
        levelText.text = levelButtonData.levelName;
    }
}
