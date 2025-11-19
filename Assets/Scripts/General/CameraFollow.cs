using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    Transform target;    // Drag the Player here in the Inspector
    public Vector3 offset;      // Set in Inspector or auto set in Start

    void Start()
    {
        target = LevelManager.instance.GetPlayer();
        if (target != null && offset == Vector3.zero)
        {
            offset = transform.position - target.position;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Camera follows the player, but keeps its own rotation
        transform.position = target.position + offset;

        // Do NOT change transform.rotation if you want it fixed
        // (leave it as it is set in the Scene)
    }
}
