using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Movement_Handler : Unit
{
    [SerializeField, Range(2, 10)] private float _force;
    [SerializeField, Range(0, 100)] private float _maxVelocityMoving;
    [SerializeField, Range(0, 100)] private float _maxVelocityHit;
    [SerializeField, Range(0, 1)] private float _minVelocityIdle;
    [SerializeField, Range(5, 15)] private float MagnitudeThreshold;
    [SerializeField, Range(1, 10)] private float slerpSpeed;
    [SerializeField] private Collider _collider;
    [SerializeField] private LayerMask whatIsNoClipLayers;
    private bool isNoClip;
    public bool IsNoClip => isNoClip;
    

    public Transform Gizmo;
    
    private bool isAbleToMove;
    
    private Vector3 xzVelocity;
    private float attackTimer = 0;
    private float maxAttackTimer = 0;

    protected override void Awake()
    {
        base.Awake();
        maxAttackTimer = unitData.AttackingStateTime;
        Level_Manager.OnLevelStart += SubToMovementEvents;
        Level_Manager.OnGameOver += UnSubToMovementEvents;
        Level_Manager.OnGameWin += UnSubToMovementEvents;
    }
    
    private void SubToMovementEvents()
    {
        Touch_Manager.OnSwipe += HandleSwipeLogic;
        isAbleToMove = true;
        SetClip(true);
    }

    private void UnSubToMovementEvents()
    {
        Touch_Manager.OnSwipe -= HandleSwipeLogic;
        isAbleToMove = false;
    }

    private void FixedUpdate()
    {
        if (Gizmo.gameObject.activeSelf && MovementState != MovementState.Moving)
        {
            Vector3 direction = Gizmo.position - transform.position;
            direction.y = 0;

            if (direction.sqrMagnitude > MagnitudeThreshold)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                Quaternion smoothedRotation = Quaternion.Slerp(_rb.rotation, targetRotation, Time.fixedDeltaTime * 5f);
                _rb.MoveRotation(smoothedRotation);
            }
        }
        
        switch (MovementState)
        {
            case MovementState.Idle:
                break;
                
            case MovementState.Attack:
                attackTimer += Time.fixedDeltaTime;
                StatHandler.SetMagnitude(StatHandler.CurrentMagnitude - Time.fixedDeltaTime * 5f);
                Debug.Log("Player's magnitude: " + StatHandler.CurrentMagnitude);

                if (attackTimer >= maxAttackTimer)
                {
                    _movementState = MovementState.Moving;
                }
                break;
            case MovementState.GotHit:
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

    public void ResetPlayer()
    {
        StatHandler.ResetStats();
        Stat_Handler.PlayerTookDamage?.Invoke(StatHandler.CurrentPercent);
        transform.position = new Vector3(0, 0.5f, 0);
        _rb.linearVelocity = Vector3.zero;
        _movementState = MovementState.Idle;
    }

    private void HandleSwipeLogic(Vector2 direction, float magnitude)
    {
        if (!isAbleToMove) return;
        if ((_movementState == MovementState.Idle && _rb.linearVelocity.magnitude <= _minVelocityIdle ) || _movementState == MovementState.GotHit)
        {
            StatHandler.SetMagnitude(magnitude);
            
            _rb.AddForce(new Vector3(direction.x, 0, direction.y) * (magnitude * _force), ForceMode.Impulse);
            _movementState = MovementState.Attack;
            attackTimer = 0;
        }
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (hasCollided) return; // Prevent duplicate triggers
        hasCollided = true;
        
        Debug.Log("Player's movement state: " + _movementState);

        var otherUnit = other.gameObject.GetComponent<Unit>();
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (_movementState == MovementState.Attack)
            {
                Debug.Log("Player hit enemy");
                if (otherUnit)
                    Collision_Manager.InvokeUnitCollision(this, otherUnit);
            }
            else if (otherUnit && _movementState != MovementState.GotHit) // Add check for otherUnit
            {
                StatHandler.TakeDamage(otherUnit.StatHandler.Damage / 2, OtherType.Player);

                var knockbackDirection = (transform.position - other.transform.position).normalized;
                float smallKnockback = otherUnit.StatHandler.Knockback;

                _rb.AddForce(knockbackDirection * smallKnockback, ForceMode.Impulse);
                _movementState = MovementState.GotHit;
            }
        }

        // Reset the flag after a short delay (e.g., if it should detect collisions again)
        StartCoroutine(ResetCollisionFlag());
    }

    public void SetClip(bool clip)
    {
        _collider.excludeLayers = whatIsNoClipLayers;
        isNoClip = clip;
    }
}