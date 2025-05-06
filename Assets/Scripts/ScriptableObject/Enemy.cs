using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Scriptable Objects/Enemy")]
public class Enemy : ScriptableObject
{
    [SerializeField, Range(0, 100)] private float percentage;
    [SerializeField, Range(0, 10)] private float speed;
    [SerializeField, Range(0, 50)] private float size;
    [SerializeField, Range(0, 50)] private float knockback;

    [SerializeField, Range(0, 20)] private float attackCoolDown;
    [SerializeField, Range(0, 30)] private float attackDamage;

    [SerializeField] private bool ranged;
    [SerializeField] private Color color;
}