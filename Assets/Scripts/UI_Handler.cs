using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class UI_Handler : MonoBehaviour
{
    [SerializeField] private Touch_Manager Touch_Manager;
    [SerializeField] private Line_Handler line;
    [SerializeField] private int _gizmoRadius;

    [SerializeField]private Transform[] points;

    private bool IsTouched;

    void OnEnable()
    {
        Touch_Manager.TouchPressAction.started += TouchStarted;
        Touch_Manager.TouchPressAction.performed += TouchStarted;
        Touch_Manager.TouchPressAction.canceled += TouchEnded;
    }

    private void Start()
    {
        line.SetUpLine(points);

    }

    void FixedUpdate()
    {
        if (Touch_Manager.InRadius)
        {
            Vector2 screenPosition = Touch_Manager.TouchPositionAction.ReadValue<Vector2>();


            points[1].gameObject.SetActive(true);

            Ray ray = Camera.main.ScreenPointToRay(screenPosition);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero); // y=0 plane

            if (groundPlane.Raycast(ray, out float enter))
            {
                Vector3 offset = ((ray.GetPoint(enter) - points[0].position) * Touch_Manager.Magnitude) / _gizmoRadius * -1;
                offset.y = points[0].transform.position.y;

                points[1].position = points[0].position + offset;
            }
        }
    }

    private void TouchStarted(InputAction.CallbackContext obj)
    {
        IsTouched = true;
    }

    private void TouchEnded(InputAction.CallbackContext obj)
    {
        points[1].position = points[0].transform.position;
        points[1].gameObject.SetActive(false);
        IsTouched = false;
    }
}