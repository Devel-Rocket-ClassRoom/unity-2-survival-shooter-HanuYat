using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    public float maxHp = 100f;
    public float damage = 20f;
    public float speed = 2f;
}