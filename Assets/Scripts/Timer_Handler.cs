using UnityEngine;
using TMPro;
using DG.Tweening;

public class Timer_Handler : MonoBehaviour
{
    //[SerializeField] private GameObject _timer;
    [SerializeField] private TMP_Text _text;
    private static float levelTime;
    private float nextSecond = 1f;
    private float nextTenSecond = 10f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        levelTime = 0;
        if (_text == null) _text = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        levelTime += Time.deltaTime;
        int minutes = (int)(levelTime / 60);
        int seconds = (int)(levelTime % 60);
        string timer = string.Format("{0:00}:{1:00}", minutes, seconds);
        _text.text = timer;

        // Per second feedback
        if (levelTime >= nextSecond)
        {
            nextSecond = Mathf.Floor(levelTime) + 1;

            // Shake effect
            _text.rectTransform.DOShakeAnchorPos(0.5f, 10f, 20, 90, false, true);

            // Alpha fade
            _text.DOFade(0.5f, 0.15f).SetLoops(2, LoopType.Yoyo);
        }

        // Every 10 seconds
        if (levelTime >= nextTenSecond)
        {
            nextTenSecond += 10;

            // Scale punch
            _text.rectTransform.DOPunchScale(Vector3.one * 0.3f, 0.4f, 6, 0.8f);
        }
    }

    public static void SetLevelTimer(int time) => levelTime = time;
}
