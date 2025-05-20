using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Touch_Manager : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;
    //[SerializeField] private GameObject _player;
    [SerializeField] private int _radiusInPixels;

    [SerializeField, Range(0,100)] private float _swipeThreshold;
    [SerializeField, Range(0,100)] private float _swipeDamping;

    private Vector3 _playerScreenPos;
    private Camera _cameraMain;
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
        _cameraMain = Camera.main;
        touchPositionAction = _playerInput.actions["Touch_Position"];
        touchPressAction = _playerInput.actions["Touch_Press"];
    }

    private void Update()
    {
        _playerScreenPos = _cameraMain.WorldToScreenPoint(Player_Manager.Instance.MovementHandler.transform.position);
    }

    private void OnEnable()
    {
        touchPressAction.started += TouchPressed;
        //touchPressAction.performed += TouchPressed;
        touchPressAction.canceled += TouchCanceled;
    }

    private void OnDisable()
    {
        touchPressAction.started -= TouchPressed;
        //touchPressAction.performed -= TouchPressed;
        touchPressAction.canceled -= TouchCanceled;
    }

    private void TouchPressed(InputAction.CallbackContext context)
    {
        performedTouchPos = touchPositionAction.ReadValue<Vector2>();

        float distance = Vector2.Distance(new Vector2(_playerScreenPos.x, _playerScreenPos.y), performedTouchPos);

        if (distance <= _radiusInPixels && Player_Manager.Instance.MovementHandler.MovementState == MovementState.Idle)
        {
            inRadius = true;
        }
        else
        {
            inRadius = false;
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
            inRadius = false;
        }
    }

    private void InvokeOnSwipe()
    {
        OnSwipe?.Invoke(swipeDirection, magnitude);
    }
    
}
