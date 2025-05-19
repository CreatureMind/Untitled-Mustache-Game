using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Handler : MonoBehaviour
{
    [SerializeField] private Touch_Manager Touch_Manager;
    [SerializeField] private Line_Handler line;
    [SerializeField] private int _gizmoRadius;

    [SerializeField]private Transform[] points;

    [Header("<allcaps><u>Percentage:")]
    [SerializeField] private TMP_Text _percentText;
    [SerializeField, Range(0, 999)] private int maxPercent;
    [SerializeField] private Color[] _percentColors;
    private float _currentFontSize;

    [Header("<allcaps><u>Lives:")]
    [SerializeField] private GameObject _heartImage;
    [SerializeField] private List<GameObject> _heartImages;
    [SerializeField] private GameObject _livesPanel;
    [SerializeField] private int _maxLives;
    
    [Header("<allcaps><u>Popups:")]
    [SerializeField] private GameObject _playerUI;
    [SerializeField] private GameObject _startPopup;
    [SerializeField] private GameObject _endPopup;
    [SerializeField] private TMP_Text _infoText;
    private int _currentDifficulty;

    private Camera _mainCamera;

    private void OnEnable()
    {
        Unit.EnemyTookDamage += EnemyUIPercentageUpdate;
        Unit.PlayerTookDamage.AddListener(PlayerUIPercentageUpdate);

        Level_Manager.OnGameWin += GameEndWin;
        Level_Manager.OnGameOver += GameEndLose;
    }

    private void OnDisable()
    {
        Unit.EnemyTookDamage -= EnemyUIPercentageUpdate;
        Unit.PlayerTookDamage.RemoveListener(PlayerUIPercentageUpdate);
    }

    private void Start()
    {
        _mainCamera = Camera.main;
        _currentFontSize = _percentText.fontSize;

        MainMenu();
        
        for (int i = 0; i < _maxLives; i++)
        {
            _heartImages.Add(Instantiate(_heartImage, _livesPanel.transform));
            _heartImages[i].SetActive(false);
        }

        SetLives();
    }

    void FixedUpdate()
    {
        //points[0] = Player, points[1] = Gizmo
        if (Touch_Manager.InRadius)
        {
            Vector3 screenPosition = Touch_Manager.TouchPositionAction.ReadValue<Vector2>();
            screenPosition.z = Vector3.Distance(_mainCamera.transform.position, points[0].position);

            points[1].gameObject.SetActive(true);

            Vector3 worldPosition = _mainCamera.ScreenToWorldPoint(screenPosition);

            Vector3 offset = (worldPosition - points[0].position).normalized * (_gizmoRadius * -1);
            offset.y = 0;

            Vector3 targetPosition = points[0].position + offset;

            points[1].position = Vector3.Lerp(points[1].position, targetPosition, Time.deltaTime * Vector3.Distance(points[0].position, worldPosition));
        }
        else
        {
            points[1].position = points[0].transform.position;
            points[1].gameObject.SetActive(false);
        }

        line.SetUpLine(points);
    }

    private void SetLives()
    {
        for (int i = 0; i < Player_Manager.Instance.StatHandler.Health; i++)
        {
            _heartImages[i].SetActive(true);
        }
    }
    private void RemoveHeart()
    {
        foreach (var heart in _heartImages)
        {
            if (heart.activeInHierarchy)
            {
                heart.SetActive(false);
                break;
            }
        }
    }

    private void AddHeart()
    {
        int i = 0;
        foreach (var heart in _heartImages)
        {
            if (heart.activeInHierarchy)
            {
                i++;
            }
        }

        if (i >= _heartImages.Count)
        {
            _heartImages.Add(Instantiate(_heartImage, _livesPanel.transform));
        }
        else
        {
            _heartImages[i].SetActive(true);
        }
    }

    public void MainMenu()
    {
        _playerUI.SetActive(false);
        _endPopup.SetActive(false);
        _startPopup.SetActive(true);
        Time.timeScale = 0;
    }

    public void StartGame(int difficulty)
    {
        _startPopup.SetActive(false);
        _playerUI.SetActive(true);
        
        _currentDifficulty = difficulty;

        switch (difficulty)
        {
            case 0:
                Level_Manager.Instance.StartLevel(Difficulty.Easy);
                break;
            case 1:
                Level_Manager.Instance.StartLevel(Difficulty.Medium);
                break;
            case 2:
                Level_Manager.Instance.StartLevel(Difficulty.Hard);
                break;
            default:
                Level_Manager.Instance.StartLevel(Difficulty.Easy);
                break;
        }
    }

    private void GameEndWin()
    {
        Time.timeScale = 0;
        _playerUI.SetActive(false);
        _infoText.text = "You beat them all! You won!";
        _endPopup.SetActive(true);
    }

    private void GameEndLose()
    {
        Time.timeScale = 0;
        _playerUI.SetActive(false);
        _infoText.text = "You Died! Try Again?";
        _endPopup.SetActive(true);
    }
    
    public void RestartLevel()
    {
        _endPopup.SetActive(false);
        _playerUI.SetActive(true);
        Level_Manager.Instance.StartLevel((Difficulty)_currentDifficulty);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private Color EnemyUIPercentageUpdate(int currentPercent)
    {
        int segmentCount = _percentColors.Length - 1;
        
        float progress = Mathf.Abs((float)currentPercent / maxPercent * segmentCount);
        int segmentIndex = (int)Mathf.Floor(progress);
        float lerpValue = progress - segmentIndex;
        
        if(segmentIndex < _percentColors.Length - 1)
        {
            return Color.Lerp(_percentColors[segmentIndex], _percentColors[segmentIndex + 1], lerpValue);
        }
        
        return _percentColors[^1];
    }
    
    private void PlayerUIPercentageUpdate(int currentPercent)
    {
        int segmentCount = _percentColors.Length - 1;
        
        _percentText.text = currentPercent + "<size=60%>%";
        
        float progress = Mathf.Abs((float)currentPercent / maxPercent * segmentCount);
        int segmentIndex = (int)Mathf.Floor(progress);
        float lerpValue = progress - segmentIndex;
        
        if(segmentIndex < _percentColors.Length - 1)
        {
            _percentText.color = Color.Lerp(_percentColors[segmentIndex], _percentColors[segmentIndex + 1], lerpValue);
            _percentText.fontSizeMax = Mathf.Lerp(_currentFontSize, _currentFontSize * 1.5f, progress / segmentCount);
        }
        else
        {
            _percentText.color = _percentColors[^1];
            _percentText.fontSizeMax = _currentFontSize * 1.5f;
        }
    }
}