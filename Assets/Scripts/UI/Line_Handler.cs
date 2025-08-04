using UnityEngine;

public class Line_Handler : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;
    private Transform[] _points;

    public void SetUpLine(Transform[] points)
    {
        _lineRenderer.positionCount = points.Length;
        _points = points;
    }

    public void Update()
    {
        if (_points == null) return;
        for (int i = 0; i < _points.Length; i++)
        {
            _lineRenderer.SetPosition(i, _points[i].position);
        }
    }
}