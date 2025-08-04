using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Handler : Base_Menu
{
    [SerializeField] private Timer_Handler timerHandler;
    [SerializeField] private Touch_Manager Touch_Manager;
    [SerializeField] private Line_Handler line;
    [SerializeField] private int _gizmoRadius;

    [SerializeField] private Transform[] points;

    [Header("<allcaps><u>Percentage:")] [SerializeField]
    private TMP_Text _percentText;

    [SerializeField, Range(0, 999)] private int maxPercent;
    [SerializeField] private Color[] _percentColors;
    private float _currentFontSize;

    [Header("<allcaps><u>Lives:")] [SerializeField]
    private GameObject _heartImage;

    private List<GameObject> _heartImages = new List<GameObject>();
    [SerializeField] private GameObject _livesPanel;
    [SerializeField] private int _maxLives;

    [Header("<allcaps><u>Popups:")] 
    [SerializeField] private Button pauseButton;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;
    
    [Header("<allcaps><u>God Panel:")] 
    [SerializeField] private Button godModeButton;
    [SerializeField] private int maxGodButtonPressCount = 5;
    [SerializeField] private GameObject godPanel;
    [SerializeField] private Button addHeartButton;
    [SerializeField] private Button removeHeartButton;
    [SerializeField] private Button noClipButton;
    [SerializeField] private Button resetTimer;
    private int godButtonPressCount = 0;
    private bool isGodModeActive = false;
    
    private int _currentDifficulty;
    
    public static Action<Color> EnemyPercentageUpdate;

    private Camera _mainCamera;

    private void OnEnable()
    {
        Stat_Handler.EnemyTookDamage += EnemyUIPercentageUpdate;
        Stat_Handler.PlayerTookDamage += PlayerUIPercentageUpdate;
        Stat_Handler.PlayerDiedButNotGameOver += SetLives;

        Level_Manager.OnLevelStart += SetLives;
    }

    private void OnDisable()
    {
        Stat_Handler.EnemyTookDamage -= EnemyUIPercentageUpdate;
        Stat_Handler.PlayerTookDamage -= PlayerUIPercentageUpdate;
    }

    private void Awake()
    {
        //pause menu button logic
        pauseButton.onClick.AddListener(PauseGame);
        resumeButton.onClick.AddListener(ResumeGame);
        restartButton.onClick.AddListener(() =>
        {
            Level_Manager.Instance.ResetLevel();
            Level_Manager.Instance.StartLevel();
            timerHandler.ResetTimer();
            timerHandler.StartTimer();
            ResumeGame();
        });
        quitButton.onClick.AddListener(() => 
        { 
            Level_Manager.Instance.ResetLevel();
            Level_Manager.Instance.PauseGame();
            Menu_Manager.Instance.SwitchMenu(MenuState.Title);
        });
        
        //god mode button logic
        godModeButton.onClick.AddListener(GodModeButtonLogic);
        
        addHeartButton.onClick.AddListener(() => { 
            Player_Manager.Instance.MovementHandler.StatHandler.Heal();
            SetLives();
        });
        
        removeHeartButton.onClick.AddListener(() =>
        {
            Player_Manager.Instance.MovementHandler.StatHandler.PlayerDied();
        });
        noClipButton.onClick.AddListener(NoClipLogic);
        resetTimer.onClick.AddListener(ResetTimerUI);

        ToggleGodMode();
    }

    private void ToggleGodMode()
    {
        godPanel.SetActive(isGodModeActive);
    }

    private void NoClipLogic()
    {
        Player_Manager.Instance.MovementHandler.SetClip(!Player_Manager.Instance.MovementHandler.IsNoClip);
    }

    private void GodModeButtonLogic()
    {
        godButtonPressCount += 1;
        if (godButtonPressCount >= maxGodButtonPressCount)
        {
            isGodModeActive = !isGodModeActive;
            godPanel.SetActive(isGodModeActive);
            godButtonPressCount = 0;
        }
    }

    protected override void OnMenuOpen()
    {
        base.OnMenuOpen();
        Level_Manager.Instance.ResumeGame();
        pauseMenu.SetActive(false);
        ResetTimerUI();
    }

    private void ResetTimerUI()
    {
        timerHandler.ResetTimer();
        timerHandler.StartTimer();
    }

    private void Start()
    {
        _mainCamera = Camera.main;
        _currentFontSize = _percentText.fontSize;

        for (int i = 0; i < _maxLives; i++)
        {
            _heartImages.Add(Instantiate(_heartImage, _livesPanel.transform));
            _heartImages[i].SetActive(false);
        }
        SetLives();
    }

    void FixedUpdate()
    {
        line.SetUpLine(points);

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

            points[1].position = Vector3.Lerp(points[1].position, targetPosition,
                Time.deltaTime * Vector3.Distance(points[0].position, worldPosition));
        }
        else
        {
            points[1].position = points[0].transform.position;
            points[1].gameObject.SetActive(false);
        }
    }

    private void SetLives()
    {
        foreach (var heart in _heartImages)
        {
            heart.SetActive(false);
        }

        for (int i = 0; i < Player_Manager.Instance.MovementHandler.StatHandler.Health; i++)
        {
            _heartImages[i].SetActive(true);
        }
    }

    private void PauseGame()
    {
        Level_Manager.Instance.PauseGame();
        pauseMenu.SetActive(true);
    }

    private void ResumeGame()
    {
        Level_Manager.Instance.ResumeGame();
        pauseMenu.SetActive(false);
    }

    private void EnemyUIPercentageUpdate(int currentPercent)
    {
        int segmentCount = _percentColors.Length - 1;

        float progress = Mathf.Abs((float)currentPercent / maxPercent * segmentCount);
        int segmentIndex = (int)Mathf.Floor(progress);
        float lerpValue = progress - segmentIndex;

        if (segmentIndex < _percentColors.Length - 1)
        {
            EnemyPercentageUpdate?.Invoke(Color.Lerp(_percentColors[segmentIndex], _percentColors[segmentIndex + 1],
                lerpValue));
        }

        EnemyPercentageUpdate?.Invoke(_percentColors[^1]);
    }

    private void PlayerUIPercentageUpdate(int currentPercent)
    {
        int segmentCount = _percentColors.Length - 1;

        _percentText.text = currentPercent + "<size=60%>%";

        float progress = Mathf.Abs((float)currentPercent / maxPercent * segmentCount);
        int segmentIndex = (int)Mathf.Floor(progress);
        float lerpValue = progress - segmentIndex;

        if (segmentIndex < _percentColors.Length - 1)
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