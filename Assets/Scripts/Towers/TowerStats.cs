using UnityEngine;

public class TowerStats : MonoBehaviour
{
    public float range = 5f;       // Detection radius of the tower
    public float fireRate = 1f;    // How many shots per second
    public Transform firePoint;    // Transform from which bullets are instantiated
    public int minDamage;
    public int maxDamage;
    public float hp;
    public float bulletSpeed;

    public int GetRandomDamage()
    {
        return Random.Range(minDamage, maxDamage);
    }
}
