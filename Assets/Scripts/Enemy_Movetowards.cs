using System.Collections;
using UnityEngine; 

public class Enemy_Movetowards : Unit
{
    private Transform _target;
    
    [SerializeField, Range(0, 10)] private float _speed;
    [SerializeField, Range(0, 10)] private float coolDown;
    [SerializeField, Range(0, 50)] private float attackForce;

    private Coroutine attackCoroutine = null;
    
    private void Start()
    {
        //_movementState = MovementState.Moving;
        _target = Player_Manager.Instance.transform;
    }
    
    void Update()
    {
        var step = _speed * Time.deltaTime;

        transform.LookAt(_target);
        transform.position = Vector3.MoveTowards(transform.position, _target.position, step);

        if (Vector3.Distance(transform.position, _target.position) < 3f)
        {
            if(attackCoroutine == null)
            {
                attackCoroutine = StartCoroutine(Attack());
            }
        }
        else
        {
            _speed = 2;
        }
    }

    private IEnumerator Attack()
    {
        yield return new WaitForSeconds(UnitData.BuildUpTime);
        if (Vector3.Distance(transform.position, _target.position) < 3f)
        {
            _speed = 0;
            var direction = _target.position - transform.position;
            _rb.AddForce(new Vector3(direction.x, 0, direction.z) * attackForce, ForceMode.Impulse);
            _movementState = MovementState.Attack;
            yield return new WaitForSeconds(unitData.AttackingStateTime);
            _movementState = MovementState.Moving;
        
            yield return new WaitForSeconds(coolDown);
            attackCoroutine = null;
        }
        else
        {
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
