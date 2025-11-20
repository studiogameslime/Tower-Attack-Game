using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int attackRange;
    public int minDamage;
    public int maxDamage;
    public int attackSpeed;
    public int health;

    public int GetRandomDamage()
    {
        return Random.Range(minDamage, maxDamage);
    }
}
