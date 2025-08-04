using UnityEngine;
using UnityEngine.UI;
public class Level_Button : MonoBehaviour
{
    [SerializeField] private Button levelButton;
    private Level_Button_Data levelButtonData;
    public Level_Button_Data LevelButtonData => levelButtonData;
    
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
    
    private string mapImagePath;
    
    private bool isNormalDifficultySelected = true;
    public bool IsNormalDifficultySelected => isNormalDifficultySelected;

    public void InitializeLevelButton(Level_Button_Data levelButtonData , int starsEarned = 0 , LevelStateType levelState = LevelStateType.Locked)
    {
        this.levelButtonData = levelButtonData;
        levelText.text = levelButtonData.levelName;
        //mapImage.sprite = Resources.Load<Sprite>(levelButtonData.mapImagePath);
        ChangeMapSprite(levelButtonData);
        SetLockedImage(levelState);
        SetStarImages(starsEarned);
        
        levelButton.onClick.AddListener(() =>
        {
            Level_Select_Menu.LevelButtonClicked(this);
            //AudioManager.Instance.PlaySound(SoundType.SFX, "Start");
        });
        
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

    private void ChangeMapSprite(Level_Button_Data levelButtonData)
    {
        mapImagePath = $"{Application.persistentDataPath}/ScreenShots/Level_{levelButtonData.levelIndex}/map.png";
        if (System.IO.File.Exists(mapImagePath))
        {
            var texture = new Texture2D(2, 2);
            byte[] fileData = System.IO.File.ReadAllBytes(mapImagePath);
            texture.LoadImage(fileData); // Load the image from file
            mapImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
        else
        {
            var path = levelButtonData.pathToMapImage;
            var sprite = Resources.Load<Sprite>(path);
        
            if (sprite == null)
            {
                Debug.LogError($"Failed to load sprite at path: Resources/{path}");
                mapImage.sprite = null;
            }
            else
            {
                mapImage.sprite = sprite;
            }
        }
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
