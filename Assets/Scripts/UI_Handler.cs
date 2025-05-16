using TMPro;
using UnityEngine;

public class UI_Handler : MonoBehaviour
{
    [SerializeField] private Touch_Manager Touch_Manager;
    [SerializeField] private Line_Handler line;
    [SerializeField] private int _gizmoRadius;

    [SerializeField]private Transform[] points;

    [Header("<allcaps><u>Percentage:")]
    [SerializeField] private TMP_Text _percentText;
    [SerializeField, Range(0, 999)] private int maxPercent;
    [Range(0, 100)] public int Percent;
    [SerializeField] private Color[] _percentColors;
    private float _currentFontSize;

    private void Start()
    {
        line.SetUpLine(points);
        _currentFontSize = _percentText.fontSize;
    }

    void FixedUpdate()
    {
        //points[0] = Player, points[1] = Gizmo
        if (Touch_Manager.InRadius)
        {
            Vector3 screenPosition = Touch_Manager.TouchPositionAction.ReadValue<Vector2>();
            screenPosition.z = Vector3.Distance(Camera.main.transform.position, points[0].position);

            points[1].gameObject.SetActive(true);

            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

            Vector3 offset = ((worldPosition - points[0].position) * Touch_Manager.Magnitude) / _gizmoRadius * -1;
            offset.y = points[0].transform.position.y;

            points[1].position = points[0].position + offset;
        }
        else
        {
            points[1].position = points[0].transform.position;
            points[1].gameObject.SetActive(false);
        }

        UIPercentageUpdate();
    }

    private void UIPercentageUpdate()
    {
        int currentPercent, segmentCount = _percentColors.Length - 1;

        //_percentText.text = CurrentPercent.ToString() + "<size=60%>%";
        _percentText.text = Percent.ToString() + "<size=60%>%";

        currentPercent = Percent;
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
            _percentText.color = _percentColors[_percentColors.Length - 1];
            _percentText.fontSizeMax = _currentFontSize * 1.5f;
        }
    }
}