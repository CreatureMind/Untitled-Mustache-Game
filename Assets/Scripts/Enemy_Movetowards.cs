using System.Collections;
using UnityEngine; 

public class Enemy_Movetowards : MonoBehaviour
{
    [SerializeField]
    private Transform _target;

    [SerializeField]
    private Rigidbody _rb;

    [SerializeField, Range(0, 10)] private float _speed;
    [SerializeField, Range(0, 10)] private float coolDown;
    [SerializeField, Range(0, 50)] private float attackForce;

    private Coroutine attackCoroutine = null;



    // Update is called once per frame
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
        _speed = 0;
        var direction = _target.position - transform.position;
        _rb.AddForce(new Vector3(direction.x, 0, direction.z) * attackForce, ForceMode.Impulse);

        yield return new WaitForSeconds(coolDown);

        attackCoroutine = null;
    }
}
