using System;
using UnityEngine;

public class Movement_Handler : Unit
{
    [SerializeField, Range(2, 10)] private float _force;

    private Vector3 xzVelocity;

    [SerializeField, Range(0, 100)] private float _maxVelocityMoving;
    [SerializeField, Range(0, 100)] private float _maxVelocityHit;
    [SerializeField, Range(0, 1)] private float _minVelocityIdle;
    
    private float attackTimer = 0;
    private float maxAttackTimer = 0;

    void Awake()
    {
        Touch_Manager.OnSwipe += HandleSwipeLogic;
        maxAttackTimer = unitData.AttackingStateTime;
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
        if ((_movementState == MovementState.Idle && _rb.linearVelocity.magnitude <= _minVelocityIdle )|| _movementState == MovementState.GotHit)
        {
            _rb.AddForce(new Vector3(direction.x, 0, direction.y) * magnitude * _force, ForceMode.Impulse);
            _movementState = MovementState.Attack;
            attackTimer = 0;

        }
    }

    private void HadleHitLogic()
    {
        _movementState = MovementState.GotHit;
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy") && _movementState == MovementState.Attack)
        {
            var otherUnit = other.gameObject.GetComponent<Unit>();
            if (otherUnit == null)
            {
                Debug.LogError("No Unit component found on the collided object.");
                return;
            }
            Collision_Manager.InvokeUnitCollision(this , otherUnit);
        }
    }
}

public enum MovementState
{
    Idle,
    Moving,
    Attack,
    GotHit
}