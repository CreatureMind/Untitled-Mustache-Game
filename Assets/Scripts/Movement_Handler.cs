using UnityEngine;

public class Movement_Handler : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Collider _collider;
    [SerializeField, Range(2, 10)] private float _force;

    private MovementState _movementState;
    public MovementState MovementState => _movementState;

    private Vector3 xzVelocity;

    [SerializeField, Range(0, 100)] private float _maxVelocityMoving;
    [SerializeField, Range(0, 100)] private float _maxVelocityHit;
    [SerializeField, Range(0, 1)] private float _minVelocityIdle;


    void Awake()
    {
        Touch_Manager.OnSwipe += HandleSwipeLogic;
    }

    private void FixedUpdate()
    {
        Debug.Log(MovementState);

        if (_rb.linearVelocity.magnitude <= _minVelocityIdle)
        {
            _movementState = MovementState.Idle;
        }
        else
        {
            _movementState = MovementState.Moving;
        }
        if (_movementState == MovementState.Moving)
        {
            if (_rb.linearVelocity.magnitude > _maxVelocityMoving)
            {
                _rb.linearVelocity = _rb.linearVelocity.normalized * _maxVelocityMoving;
            }
        }
        if (_movementState == MovementState.Hit)
        {
            if (_rb.linearVelocity.magnitude > _maxVelocityHit)
            {
                _rb.linearVelocity = _rb.linearVelocity.normalized * _maxVelocityHit;
            }
        }
    }

    private void HandleSwipeLogic(Vector2 direction, float magnitude)
    {
        if (_movementState == MovementState.Idle && _rb.linearVelocity.magnitude <= _minVelocityIdle)
        {
            _rb.AddForce(new Vector3(direction.x, 0, direction.y) * magnitude * _force, ForceMode.Impulse);
            _movementState = MovementState.Moving;
        }
    }

    private void HadleHitLogic()
    {
        _movementState = MovementState.Hit;
    }
    
    
}

public enum MovementState
{
    Idle,
    Moving,
    Hit
}