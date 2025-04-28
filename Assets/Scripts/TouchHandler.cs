using UnityEngine;
using UnityEngine.InputSystem;

public class TouchHandler : MonoBehaviour
{
    private InputControls controls;
    private Vector2 position;

    void Awake()
    {
        controls = new InputControls();
        controls.Enable();
        controls.Touch.Tap.performed += OnTouch;
    }

    public void OnTouch(InputAction.CallbackContext ctx)
    { 
        
    }
}
