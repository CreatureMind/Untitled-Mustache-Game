using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Scriptable Objects/Enemy")]
public class Unit_Data : ScriptableObject
{
    [SerializeField, Range(0, 100)] private float damage;
    [SerializeField, Range(0, 10)] private float speed;
    [SerializeField, Range(0, 100)] private float weight;
    [SerializeField, Range(0, 50)] private float knockback;

    [SerializeField, Range(0, 10)] private float attackCoolDown;

    [SerializeField, Range(0, 10)] private float attackRange;
    
    [SerializeField] private bool ranged;
    [SerializeField] private Color color;
    
    public float Damage => damage;
    public float Speed => speed;
    public float Weight => weight;
    public float Knockback => knockback;
    public float AttackCoolDown => attackCoolDown;
    public float AttackRange => attackRange;
    public bool Ranged => ranged;
    public Color Color => color;
    
}