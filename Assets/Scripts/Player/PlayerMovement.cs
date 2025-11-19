using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rigidbody;
    public Joystick joystick;
    public float playerSpeed;

    // Update is called once per frame
    private void FixedUpdate()
    {
        rigidbody.linearVelocity = new Vector3(
            joystick.Horizontal * playerSpeed,
            rigidbody.linearVelocity.y,
            joystick.Vertical * playerSpeed
        );

        // Only rotate if we actually have input
        Vector3 direction = new Vector3(joystick.Horizontal, 0f, joystick.Vertical);

        if (direction.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }
    }
}
