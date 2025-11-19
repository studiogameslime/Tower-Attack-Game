using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rigidbody;
    public Joystick joystick;
    public float playerSpeed;

    // Update is called once per frame
    private void FixedUpdate()
    {
        rigidbody.linearVelocity = new Vector3(joystick.Horizontal * playerSpeed, rigidbody.linearVelocity.y, joystick.Vertical * playerSpeed);

        if (joystick.Horizontal != 0 || joystick.Vertical != 0)
        {
            transform.rotation = Quaternion.LookRotation(rigidbody.linearVelocity);
        }
    }
}
