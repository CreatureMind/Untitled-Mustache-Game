using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class UI_Handler : MonoBehaviour
{
    [SerializeField] private Touch_Manager Touch_Manager;
    [SerializeField] private Line_Handler line;

    [SerializeField ]private Transform[] points;

    public void Awake()
    {
    }

    void OnEnable()
    {
        Touch_Manager.TouchPressAction.started += TouchStarted;
        Touch_Manager.TouchPressAction.performed += TouchStarted;
        Touch_Manager.TouchPressAction.canceled += TouchStarted;
    }

    private void TouchStarted(InputAction.CallbackContext obj)
    {
        Debug.Log(obj.phase);
    }

    private void Start()
    {
        line.SetUpLine(points);
    }
}
