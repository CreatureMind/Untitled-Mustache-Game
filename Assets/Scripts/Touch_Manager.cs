using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Touch_Manager : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private GameObject _player;
    [SerializeField] private int _radiusInPixels;

    [SerializeField, Range(0,100)] private float _swipeThreshold;
    [SerializeField, Range(0,100)] private float _swipeDamping;

    private Vector3 _playerScreenPos;
    private bool inRadius;
    public bool InRadius => inRadius;

    private InputAction touchPositionAction;
    public InputAction TouchPositionAction => touchPositionAction;
    
    private InputAction touchPressAction;
    public InputAction TouchPressAction => touchPressAction;

    private Vector2 performedTouchPos;
    private Vector2 canceledTouchPos;

    private Vector2 swipeDirection;

    private float magnitude;
    public float Magnitude => magnitude;

    public static Action<Vector2, float> OnSwipe; 

    private void Awake()
    {
        touchPositionAction = _playerInput.actions["Touch_Position"];
        touchPressAction = _playerInput.actions["Touch_Press"];
        _playerScreenPos = Camera.main.WorldToScreenPoint(_player.transform.position);
    }

    private void Update()
    {

    }

    private void OnEnable()
    {
        touchPressAction.started += TouchPressed;
        touchPressAction.canceled += TouchCanceled;
    }

    private void OnDisable()
    {
        touchPressAction.performed -= TouchPressed;
    }

    private void TouchPressed(InputAction.CallbackContext context)
    {
        performedTouchPos = touchPositionAction.ReadValue<Vector2>();

        float distance = Vector2.Distance(new Vector2(_playerScreenPos.x, _playerScreenPos.y), performedTouchPos);

        if (distance <= _radiusInPixels && _player.GetComponent<Movement_Handler>().MovementState == MovementState.Idle)
        {
            inRadius = true;
        }
    }

    private void TouchCanceled(InputAction.CallbackContext context)
    {
        canceledTouchPos = touchPositionAction.ReadValue<Vector2>();
        if (performedTouchPos != canceledTouchPos)
        {
            HandleSwipe();
        }

        inRadius = false;
    }

    private void HandleSwipe()
    {
        if (inRadius)
        {
            magnitude = Mathf.Clamp(Vector2.Distance(performedTouchPos, canceledTouchPos) / _swipeDamping, 0, _swipeThreshold);
            Debug.Log(magnitude);

            swipeDirection = (canceledTouchPos - performedTouchPos).normalized * -1; //-1 to invert
            InvokeOnSwipe();
        }
    }

    private void InvokeOnSwipe()
    {
        OnSwipe?.Invoke(swipeDirection, magnitude);
    }
    
}
