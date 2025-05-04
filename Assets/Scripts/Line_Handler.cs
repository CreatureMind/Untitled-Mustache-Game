using UnityEngine;

public class Line_Handler : MonoBehaviour
{
    [SerializeField] LineRenderer lr;
    private Transform[] points;

    public void SetUpLine(Transform[] points)
    {
        lr.positionCount = points.Length;
        this.points = points;
    }

    public void Update()
    {
        for (int i = 0; i < points.Length; i++)
        {
            lr.SetPosition(i, points[i].position);
        }
    }
}
