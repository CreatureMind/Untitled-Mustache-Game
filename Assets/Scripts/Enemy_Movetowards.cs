using System.Collections;
using UnityEngine; 

public class Enemy_Movetowards : Unit
{
    private Transform _target;
    
    private float _speed;
    [SerializeField, Range(0, 10)] private float coolDown;
    [SerializeField, Range(0, 50)] private float attackForce;

    private Coroutine attackCoroutine = null;
    
    private void Start()
    {
        //_movementState = MovementState.Moving;
        _target = Player_Manager.Instance.transform;
        _speed = unitData.Speed;
    }
    
    void Update()
    {
        var step = _speed * Time.deltaTime;

        transform.LookAt(_target);
        transform.position = Vector3.MoveTowards(transform.position, _target.position, step);

        if (Vector3.Distance(transform.position, _target.position) < unitData.AttackRange)
        {
            if(attackCoroutine == null)
            {
                attackCoroutine = StartCoroutine(Attack());
            }
        }
        else
        {
            _speed = unitData.Speed;
        }
    }

    private IEnumerator Attack()
    {
        yield return new WaitForSeconds(UnitData.BuildUpTime);
        if (Vector3.Distance(transform.position, _target.position) < 3f)
        {
            _speed = 0;
            var direction = _target.position - transform.position;
            _rb.AddForce(new Vector3(direction.x, 0, direction.z) * unitData.AttackForce, ForceMode.Impulse);
            _movementState = MovementState.Attack;
            yield return new WaitForSeconds(unitData.AttackingStateTime);
            _movementState = MovementState.Moving;
        
            yield return new WaitForSeconds(unitData.AttackCoolDown);
            attackCoroutine = null;
        }
        else
        {
            _movementState = MovementState.Moving;
            attackCoroutine = null;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player") && _movementState == MovementState.Attack)
        {
            var otherUnit = other.gameObject.GetComponent<Unit>();
            if (otherUnit == null)
            {
                return;
            }
            Collision_Manager.InvokeUnitCollision(this, otherUnit);
        }
    }
}
