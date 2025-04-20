using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof (BoxCollider))]

public class PlayerController : MonoBehaviour
{
    public Rigidbody rb; 
    public FixedJoystick joystick;
    public float SpeedMove;
    public Animator animator;

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    void Update()
    {

    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(joystick.Horizontal * SpeedMove, rb.linearVelocity.y, joystick.Vertical * SpeedMove);

        if (joystick.Horizontal != 0 || joystick.Vertical != 0)
        {
            transform.rotation = Quaternion.LookRotation(rb.linearVelocity);
            animator.SetBool("isRunning", true);
        }else{
            animator.SetBool("isRunning", false);
        }
    }
}
