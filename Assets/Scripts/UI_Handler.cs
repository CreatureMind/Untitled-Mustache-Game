using UnityEngine;

public class UI_Handler : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private Touch_Manager Touch_Manager;
    [SerializeField] private Line_Handler line;

    [SerializeField ]private Transform[] points;

    private void Start()
    {
        line.SetUpLine(points);
    }
}
