using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerAnimator2D : MonoBehaviour
{
    [Header("Idle Sit Timing")]
    [SerializeField] private float speedThreshold = 0.01f;

    private Animator anim;
    private Rigidbody2D rb;

    private float idleTime;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Speed (for walk/idle transitions)
        float speed = Mathf.Abs(rb.velocity.x);
        anim.SetFloat("Speed", speed);

        // IdleTime (for stand -> sit transitions)
        if (speed <= speedThreshold)
            idleTime += Time.deltaTime;
        else
            idleTime = 0f;

        anim.SetFloat("IdleTime", idleTime);
    }
}