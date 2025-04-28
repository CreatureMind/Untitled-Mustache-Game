using UnityEngine;
using UnityEngine.InputSystem;

public class Touch_Manager : MonoBehaviour
{
    [SerializeField]
    private GameObject _player;

    private PlayerInput _playerInput;

    private InputAction touchPositionAction;
    private InputAction touchPressAction;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        touchPositionAction = _playerInput.actions.FindAction("Touch_Position");
        touchPressAction = _playerInput.actions.FindAction("Touch_Press");
    }

    private void OnEnable()
    {
        touchPressAction.performed += TouchPressed;
    }

    private void OnDisable()
    {
        touchPressAction.performed -= TouchPressed;
    }

    private void TouchPressed(InputAction.CallbackContext context)
    {
        Vector2 screenPosition = touchPositionAction.ReadValue<Vector2>();

        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 targetPosition = hit.point;
            targetPosition.y = _player.transform.position.y;

            _player.transform.position = targetPosition;
        }
    }
}
