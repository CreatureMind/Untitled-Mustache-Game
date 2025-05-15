using UnityEngine;

public class Movement_Handler : Unit
{
    [SerializeField] private Collider _collider;
    [SerializeField, Range(2, 10)] private float _force;

    private MovementState _movementState;
    public MovementState MovementState => _movementState;

    private Vector3 xzVelocity;

    [SerializeField, Range(0, 100)] private float _maxVelocityMoving;
    [SerializeField, Range(0, 100)] private float _maxVelocityHit;
    [SerializeField, Range(0, 1)] private float _minVelocityIdle;
    
    private float attackTimer = 0;
    [SerializeField] private float maxAttackTimer = 0;


    void Awake()
    {
        Touch_Manager.OnSwipe += HandleSwipeLogic;
    }

    private void FixedUpdate()
    {
        Debug.Log(MovementState);
        switch (MovementState)
        {
            case MovementState.Idle:
                break;
                
            case MovementState.Attack:
                attackTimer += Time.deltaTime;
                if (attackTimer >= maxAttackTimer)
                {
                    _movementState = MovementState.Moving;
                }
                break;
            
            case MovementState.Moving:
                if (_rb.linearVelocity.magnitude <= _minVelocityIdle)
                {
                    _movementState = MovementState.Idle;
                }
                break;
        }
        if (_rb.linearVelocity.magnitude >= _maxVelocityMoving)
        {
            _rb.linearVelocity = _rb.linearVelocity.normalized * _maxVelocityMoving;
        }
    }

    private void HandleSwipeLogic(Vector2 direction, float magnitude)
    {
        if (_movementState == MovementState.Idle && _rb.linearVelocity.magnitude <= _minVelocityIdle)
        {
            _rb.AddForce(new Vector3(direction.x, 0, direction.y) * magnitude * _force, ForceMode.Impulse);
            _movementState = MovementState.Attack;
            attackTimer = 0;

        }
    }

    private void HadleHitLogic()
    {
        _movementState = MovementState.Hit;
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy") && _movementState == MovementState.Attack)
        {
            Collision_Manager.InvokePlayerAttack(this);
        }
    }
}

public enum MovementState
{
    Idle,
    Moving,
    Attack,
    Hit
}