using UnityEngine;
using UnityEngine.UI;
public class Level_Button : MonoBehaviour
{
    [SerializeField] private Button levelButton;
    
    [SerializeField] private Button normalDifficultyButton;
    [SerializeField] private Button infiniteDifficultyButton;
    
    [SerializeField] private TMPro.TextMeshProUGUI levelText;
    
    [SerializeField] private Image mapImage;
    [SerializeField] private GameObject LockedImage;
    [SerializeField] private GameObject LockedInfinityImage;
    
    [SerializeField] private Image[] stars;
    
    [SerializeField] private Sprite selectedImage;
    [SerializeField] private Sprite unSelectedImage;
    
    [SerializeField] private Sprite emptyStar;
    [SerializeField] private Sprite filledStar;
    
    private bool isNormalDifficultySelected = true;

    public void InitalizeLevelButton(Level_Button_Data levelButtonData)
    {
        levelText.text = levelButtonData.levelName;
        SetLockedImage(levelButtonData.levelStateType);
        SetStarImages(levelButtonData.starsEarned);
        
        
        normalDifficultyButton.onClick.AddListener(() =>
        {
            isNormalDifficultySelected = true;
            normalDifficultyButton.image.sprite = selectedImage;
            infiniteDifficultyButton.image.sprite = unSelectedImage;
        });
        infiniteDifficultyButton.onClick.AddListener(() =>
        {
            isNormalDifficultySelected = false;
            normalDifficultyButton.image.sprite = unSelectedImage;
            infiniteDifficultyButton.image.sprite = selectedImage;
        });
    }

    private void SetLockedImage(LevelStateType levelStateType)
    {
        switch (levelStateType)
        {
            case LevelStateType.Locked:
                LockedImage.SetActive(true);
                normalDifficultyButton.interactable = false;
                infiniteDifficultyButton.interactable = false;
                levelButton.interactable = false;
                break;
            case LevelStateType.Normal:
                LockedImage.SetActive(false);
                normalDifficultyButton.interactable = true;
                isNormalDifficultySelected = true;
                infiniteDifficultyButton.interactable = false;
                LockedInfinityImage.SetActive(true);
                levelButton.interactable = true;
                break;
            case LevelStateType.Infinite:
                LockedImage.SetActive(false);
                normalDifficultyButton.interactable = true;
                infiniteDifficultyButton.interactable = true;
                LockedInfinityImage.SetActive(false);
                levelButton.interactable = true;
                break;
        }
    }

    private void SetStarImages(int starsEarned)
    {
        for (int i = 0; i < stars.Length; i++)
        {
            if (i < starsEarned)
            {
                stars[i].sprite = filledStar;
            }
            else
            {
                stars[i].sprite = emptyStar;
            }
        }
    }
}
