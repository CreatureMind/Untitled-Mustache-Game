using UnityEngine;
using UnityEngine.UI;

public class Lose_Menu : Base_Menu
{
    [SerializeField] private Button restartButton;
    [SerializeField] private Button ReturnButton;

    private void Start()
    {
        restartButton.onClick.AddListener(Level_Manager.Instance.StartLevel);
        ReturnButton.onClick.AddListener(() =>
            {
                Menu_Manager.Instance.SwitchMenu(MenuState.LevelSelect); 
            }
            );
    }
}

