using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    public Rigidbody rb;
    public FixedJoystick joystick;
    public float speedMove = 5f;
    public Animator animator;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // 防止物理影响旋转
    }

    private void FixedUpdate()
    {
        Vector3 move = new Vector3(joystick.Horizontal, 0f, joystick.Vertical);

        if (move.magnitude > 0.1f)
        {
            Vector3 velocity = move.normalized * speedMove;
            rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);

            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);

            animator.SetBool("isRunning", true);
        }
        else
        {
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            animator.SetBool("isRunning", false);
        }
    }
}