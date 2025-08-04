using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Win_Menu : Base_Menu
{
    [SerializeField] private Button tryAgainButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private List<Image> stars;

    [SerializeField] private Sprite starSprite;
    [SerializeField] private Sprite emptyStarSprite;

    private void Start()
    {
        tryAgainButton.onClick.AddListener(Level_Manager.Instance.StartLevel);
        nextButton.onClick.AddListener(() => { Menu_Manager.Instance.SwitchMenu(MenuState.LevelSelect); }
        );
    }
    
    public void SetStars(int starsCount)
    {
        for (var i = 0; i < stars.Count; i++)
        {
            stars[i].sprite = i < starsCount ? starSprite : emptyStarSprite;
        }

        tryAgainButton.gameObject.SetActive(starsCount != 3);
    }
}