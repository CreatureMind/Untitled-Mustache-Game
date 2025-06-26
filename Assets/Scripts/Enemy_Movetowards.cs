using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Enemy_Movetowards : Unit
{
    private Transform _target;
    private float _speed;
    private float _distanceToTarget;

    [SerializeField] private Image percentImage;

    private Coroutine _attackCoroutine = null;

    private void OnEnable()
    {
        UI_Handler.EnemyPerentageUpdate += PercentImageUpdate;   
    }

    private void OnDisable()
    {
        UI_Handler.EnemyPerentageUpdate -= PercentImageUpdate;
    }

    private void Start()
    {
        _target = Player_Manager.Instance.transform;
        _speed = unitData.Speed;

        StartCoroutine(SetPhysics());
    }
    
    void FixedUpdate()
    {
        _distanceToTarget = Vector3.Distance(transform.position, _target.position); // Distance to the player
        var step = _speed * Time.fixedDeltaTime; // Movement step size

        switch (_movementState)
        {
            case MovementState.Moving:
                if (_distanceToTarget <= unitData.AttackRange)
                {
                    // Stop moving if within attack range
                    //_speed = 0; // Stop movement speed
                    _movementState = MovementState.Idle;
                }
                else
                {
                    // Move towards the target
                    _speed = unitData.Speed; // Set speed to normal moving speed
                    transform.LookAt(_target); // Face the player
                    _rb.MovePosition(Vector3.MoveTowards(transform.position, _target.position, step));
                }
                break;

            case MovementState.Idle:
                SlowDownOverTime();
                transform.LookAt(_target); // Face the player
                
                if (_speed > 0) // Keep moving if there's still speed
                {
                    _rb.MovePosition(Vector3.MoveTowards(transform.position, _target.position, step));
                }

                if (_distanceToTarget > unitData.AttackRange)
                {
                    // Transition back to moving if the player is out of range
                    _movementState = MovementState.Moving;
                    _speed = unitData.Speed;
                }
                else if (_attackCoroutine == null)
                {
                    // If within range, try attacking
                    _attackCoroutine = StartCoroutine(Attack());
                }
                break;

            case MovementState.Attack:
                // Speed remains 0 during attack, handled within the coroutine
                break;

            case MovementState.GotHit:
                // When the enemy gets hit, you can optionally slow down or stop its movement:
                _speed = 0; // Stop movement in GotHit state
                if (_rb.linearVelocity.magnitude <= 0)
                {
                    _movementState = MovementState.Idle;
                }
                break;

            default:
                _movementState = MovementState.Moving; // Default to moving
                break;
        }
    }

    private void SlowDownOverTime()
    {
        // Smoothly reduce speed while still moving slightly
        _speed = Mathf.Lerp(_speed, 0, Time.deltaTime * 3); // Adjust the multiplier (2) for faster/slower deceleration

        if (_speed < 0.01f)
        {
            // Fully stop once the speed is near zero
            _speed = 0;
        }
    }

    private IEnumerator Attack()
    {
        Debug.Log("Enemy preparing to attack!");

        // Build up (attack preparation phase)
        yield return new WaitForSeconds(UnitData.BuildUpTime);
        
        if (_distanceToTarget < unitData.AttackRange)
        {
            // If still within range, perform the attack
            _movementState = MovementState.Attack;
            
            _speed = 0;
            var direction = (_target.position - transform.position).normalized;
            StatHandler.SetMagnitude(Mathf.Clamp(Vector2.Distance(transform.position, direction), 5, 10));
            var magnitude = StatHandler.CurrentMagnitude;
            Debug.Log("Enemy's magnitude: " + StatHandler.CurrentMagnitude);
            
            //float randomForce = UnityEngine.Random.Range(2f, 5f);
            _rb.AddForce(new Vector3(direction.x, 0, direction.z) * (magnitude * unitData.AttackForce), ForceMode.Impulse);
            
            yield return new WaitForSeconds(unitData.AttackingStateTime);
        }
        else
        {
            Debug.Log("Enemy canceled attack - player out of range.");
        }
        
        _movementState = (_distanceToTarget <= unitData.AttackRange) ? MovementState.Idle : MovementState.Moving;
        
        // Cooldown period after attack
        yield return new WaitForSeconds(unitData.AttackCoolDown);
        
        _attackCoroutine = null; // Reset coroutine flag
    }

    private IEnumerator SetPhysics()
    {
        yield return new WaitForSeconds(0.5f);
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (hasCollided) return; // Prevent duplicate triggers
        hasCollided = true;
        
        Debug.Log("Enemy's movement state: " + _movementState);

        var otherUnit = other.gameObject.GetComponent<Unit>();
        if (other.gameObject.CompareTag("Player") && _movementState == MovementState.Attack)
        {
            Debug.Log("Enemy hit player");
            
            if (otherUnit != null)
            {
                Collision_Manager.InvokeUnitCollision(this, otherUnit);
            }
        }
        else if (otherUnit && _movementState != MovementState.GotHit) // Add check for otherUnit
        {
            var knockbackDirection = (transform.position - other.transform.position).normalized;
            float smallKnockback = otherUnit.StatHandler.Knockback;

            _rb.AddForce(knockbackDirection * smallKnockback, ForceMode.Impulse);
        }
        else
        {
            _movementState = MovementState.GotHit;
        }

        // Reset the flag after a short delay (e.g., if it should detect collisions again)
        StartCoroutine(ResetCollisionFlag());
    }

    private void PercentImageUpdate(Color color)
    {
        percentImage.color = color;
    }
}