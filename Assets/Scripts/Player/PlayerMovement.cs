using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rigidbody;
    public Joystick joystick;
    public float playerSpeed;
    public PlayerAnimator playerAnimator;

    private void Awake()
    {
        playerAnimator = GetComponent<PlayerAnimator>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        // Movement
        Vector3 velocity = new Vector3(
            joystick.Horizontal * playerSpeed,
            rigidbody.linearVelocity.y,
            joystick.Vertical * playerSpeed
        );

        rigidbody.linearVelocity = velocity;

        // Rotation
        Vector3 direction = new Vector3(velocity.x, 0f, velocity.z);

        if (direction.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }

        // Animation only when moving
        bool isMoving = new Vector3(velocity.x, 0, velocity.z).sqrMagnitude > 0.01f;
        playerAnimator.SetRunningAnimation(isMoving);
    }
}
