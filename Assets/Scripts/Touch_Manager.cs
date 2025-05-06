using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Touch_Manager : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private Transform _gizmo;

    [SerializeField] private PlayerInput _playerInput;
    
    [SerializeField, Range(0,100)] private float _swipeThreshold;
    [SerializeField, Range(0,100)] private float _swipeDamping;


    private InputAction touchPositionAction;
    private InputAction touchPressAction;

    private Vector2 performedTouchPos;
    private Vector2 canceledTouchPos;
    
    private Vector2 screenPosition;

    private Vector2 swipeDirection;
    public Vector2 SwipeDirection => swipeDirection;

    private float magnitude;
    public float Magnitude => magnitude;

    public static Action<Vector2, float> OnSwipe; 

    private void Awake()
    {
        touchPositionAction = _playerInput.actions["Touch_Position"];
        touchPressAction = _playerInput.actions["Touch_Press"];
    }

    private void OnEnable()
    {
        touchPressAction.performed += TouchPressed;
        touchPressAction.canceled += TouchCanceled;
    }

    private void OnDisable()
    {
        touchPressAction.performed -= TouchPressed;
    }

    private void Update()
    {
        screenPosition = touchPositionAction.ReadValue<Vector2>();

        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        Vector3 targetPosition = Vector3.zero;

        if (Physics.Raycast(ray, out hit))
        {
            targetPosition = _player.transform.position - hit.point;
            targetPosition.y = _player.transform.position.y;
            Debug.Log(targetPosition);
        }
        
        _gizmo.position = targetPosition;
    }

    private void TouchPressed(InputAction.CallbackContext context)
    {
        performedTouchPos = touchPositionAction.ReadValue<Vector2>();
    }

    private void TouchCanceled(InputAction.CallbackContext context)
    {
        canceledTouchPos = touchPositionAction.ReadValue<Vector2>();
        if (performedTouchPos != canceledTouchPos)
        {
            HandleSwipe();
        }
    }

    private void HandleSwipe()
    { 
        magnitude =  Mathf.Clamp(Vector2.Distance(performedTouchPos, canceledTouchPos) / _swipeDamping, 0, _swipeThreshold);
        
        swipeDirection = (canceledTouchPos - performedTouchPos).normalized * -1; //-1 to invert
        InvokeOnSwipe();
        //Debug.DrawRay_player.transform.position, canceledTouchPos, Color.red);
    }

    private void InvokeOnSwipe()
    {
        OnSwipe?.Invoke(swipeDirection, magnitude);
    }
    
}

/*
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 targetPosition = hit.point;
            targetPosition.y = _player.transform.position.y;

            _player.transform.position = targetPosition;
        }
        */
