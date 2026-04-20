using UnityEngine;

[RequireComponent(typeof(LuckSystem))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerAbilities : MonoBehaviour
{
    [Header("Costs")]
    public float dashLuckCost = 15f;
    public float slowFallPerSecondCost = 8f;

    [Header("Dash")]
    public float dashForce = 14f;
    public float dashCooldown = 0.75f;
    private float lastDashTime = -999f;

    private LuckSystem luck;
    private Rigidbody2D rb;

    private void Awake()
    {
        luck = GetComponent<LuckSystem>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void TryDash(float inputX)
    {
        if (!luck.CanDash) return;
        if (Time.time < lastDashTime + dashCooldown) return;
        if (!luck.SpendLuck(dashLuckCost)) return;

        float dir = Mathf.Abs(inputX) > 0.01f ? Mathf.Sign(inputX) : transform.localScale.x;
        rb.velocity = new Vector2(0f, rb.velocity.y);
        rb.AddForce(new Vector2(dir * dashForce, 0f), ForceMode2D.Impulse);

        lastDashTime = Time.time;
    }

    public bool TrySlowFall(bool holdingJump)
    {
        if (!holdingJump || !luck.CanSlowFall) return false;

        float cost = slowFallPerSecondCost * Time.fixedDeltaTime;
        if (!luck.SpendLuck(cost)) return false;

        if (rb.velocity.y < -2f)
            rb.velocity = new Vector2(rb.velocity.x, -2f);

        return true;
    }
}
