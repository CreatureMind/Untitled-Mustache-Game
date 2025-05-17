using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Scriptable Objects/Unit_Data")]
public class Unit_Data : ScriptableObject
{
    [SerializeField, Range(0, 100)] private int damage;
    [SerializeField, Range(0, 10)] private float speed;
    [SerializeField, Range(0, 100)] private float weight;
    [SerializeField, Range(0, 50)] private float knockback;

    [SerializeField, Range(0, 10)] private float attackForce;
    [SerializeField, Range(0, 10)] private float attackCoolDown;
    [SerializeField, Range(0, 2)] private float attackingStateTime;
    [SerializeField, Range(0, 5)] private float buildUpTime;

    [SerializeField, Range(0, 10)] private float attackRange;
    
    [SerializeField] private bool ranged;
    [SerializeField] private Color color;
    
    public int Damage => damage;
    public float Speed => speed;
    public float Weight => weight;
    public float Knockback => knockback;
    public float AttackForce => attackForce;
    public float AttackCoolDown => attackCoolDown;
    public float AttackingStateTime => attackingStateTime;
    public float BuildUpTime => buildUpTime;
    public float AttackRange => attackRange;
    public bool Ranged => ranged;
    public Color Color => color;
    
}