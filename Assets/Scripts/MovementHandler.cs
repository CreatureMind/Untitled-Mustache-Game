using System;
using UnityEngine;

public class MovementHandler : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider collider;

    void Awake()
    {
        Touch_Manager.OnSwipe += HandleSwipeLogic;
    }

    private void HandleSwipeLogic(Vector2 direction, float magnitude)
    {
        rb.AddForce(new Vector3(direction.x, 0, direction.y) * magnitude, ForceMode.Impulse);
        Debug.Log(direction + " " + magnitude);
    }
    
    
}
