using UnityEngine;
using TMPro;
using DG.Tweening;

public class Timer_Handler : MonoBehaviour
{
    //[SerializeField] private GameObject _timer;
    [SerializeField] private TMP_Text _text;
    [SerializeField] private float levelTime;
    private float nextSecond;
    private float nextTenSecond;

    private void Start()
    {
        nextTenSecond = levelTime;
        nextSecond = levelTime;
    }

    // Update is called once per frame
    void Update()
    {
        levelTime -= Time.deltaTime;
        int minutes = (int)(levelTime / 60);
        int seconds = (int)(levelTime % 60);
        string timer = string.Format("{0:00}:{1:00}", minutes, seconds);
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

        if(levelTime <= 0)
        {
            levelTime = 0;
        }
    }

    public void SetLevelTimer(int time) => levelTime = time;
}
