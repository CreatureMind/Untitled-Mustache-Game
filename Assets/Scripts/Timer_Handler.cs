using UnityEngine;
using TMPro;
using DG.Tweening;

public class Timer_Handler : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private float levelTime;
    private int initialLevelTime;
    private float nextSecond;
    private float nextTenSecond;
    private bool isTimerActive = true;
    private bool didCapture = true;
    public static bool CanGetExtraStar = true;

    private string timer;
    int minutes;
    int seconds;

    private void Awake()
    {
        initialLevelTime = (int)levelTime;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (!isTimerActive) return;
        levelTime -= Time.deltaTime;
        minutes = (int)(levelTime / 60);
        seconds = (int)(levelTime % 60);
        timer = $"{minutes:00}:{seconds:00}";
        _text.text = timer;

        // Per second feedback
        if (levelTime <= nextSecond)
        {
            nextSecond = Mathf.Floor(levelTime) - Time.deltaTime;

            // Shake effect
            _text.rectTransform.DOShakeAnchorPos(0.5f, 10f, 20, 90, false, true);

            // Alpha fade
            _text.DOFade(0.5f, 0.15f).SetLoops(2, LoopType.Yoyo);
        }

        // Every 10 seconds
        if (levelTime <= nextTenSecond)
        {
            nextTenSecond -= 10;

            // Scale punch
            _text.rectTransform.DOPunchScale(Vector3.one * 0.3f, 0.4f, 6, 0.8f);
        }

        if (levelTime <= 100 && !didCapture)
        {
            didCapture = true;
            Capture_Handler.CaptureAction?.Invoke();
            Debug.Log("Half time reached!");
        }

        if(levelTime <= 0)
        {
            levelTime = 0;
            CanGetExtraStar = false;
        }
    }
    public void ResetTimer()
    {
        isTimerActive = false;
    }
    public void StartTimer()
    {
        isTimerActive = true;
        nextSecond = levelTime;
        nextTenSecond = levelTime;
        levelTime = initialLevelTime;
        CanGetExtraStar = true;
        didCapture = false;
    }

    public void SetLevelTimer(int time) => levelTime = time;
}
