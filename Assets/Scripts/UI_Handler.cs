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
    [Range(0, 100)] public int Percent;
    [SerializeField] private Color[] _percentColors;
    private int _currentColorIndex = 0, _targetColorIndex = 1;
    private float _targetPoint;


    private void Start()
    {
        line.SetUpLine(points);
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
        int maxLife = 100, currentLife = 0, bars = 1;

        //_percentText.text = CurrentPercent.ToString() + "<size=60%>%";
        _percentText.text = Percent.ToString() + "<size=60%>%";

        currentLife = Percent;
        _targetPoint = Mathf.Abs((float)currentLife / maxLife * bars);
        //float remainingBars = remainingLife;
        //int lostLife = bars - remainingBars;
        Debug.Log(_targetPoint);

        _percentText.color = Color.Lerp(_percentColors[_currentColorIndex], _percentColors[_targetColorIndex], _targetPoint);

        if(_targetPoint >= 1f)
        {
            _targetPoint = 0;
            _currentColorIndex = _targetColorIndex;
            _targetColorIndex++;
        }
    }
}