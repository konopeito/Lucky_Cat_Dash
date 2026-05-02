using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class MovingPlatform2D : MonoBehaviour
{
    public enum MoveAxis { Horizontal, Vertical, CustomPoints }

    [Header("Movement")]
    public MoveAxis moveAxis = MoveAxis.Horizontal;
    public float distance = 3f;
    public float speed = 2f;

    [Header("Custom Points (optional)")]
    public Transform pointA;
    public Transform pointB;

    private Vector3 _a;
    private Vector3 _b;

    private Vector3 _lastPos;
    private Transform _ridingPlayer;

    private void Start()
    {
        Vector3 startPos = transform.position;

        if (moveAxis == MoveAxis.CustomPoints && pointA != null && pointB != null)
        {
            _a = pointA.position;
            _b = pointB.position;
        }
        else
        {
            _a = startPos;
            _b = startPos + (moveAxis == MoveAxis.Vertical ? Vector3.up : Vector3.right) * distance;
        }

        _lastPos = transform.position;
    }

    private void FixedUpdate()
    {
        float t = Mathf.PingPong(Time.time * speed, 1f);
        Vector3 newPos = Vector3.Lerp(_a, _b, t);

        Vector3 delta = newPos - transform.position;
        transform.position = newPos;

        // Move rider by same delta
        if (_ridingPlayer != null)
            _ridingPlayer.position += delta;

        _lastPos = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.GetComponent<PlayerController2D>() != null)
            _ridingPlayer = col.transform;
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.collider.GetComponent<PlayerController2D>() != null && col.transform == _ridingPlayer)
            _ridingPlayer = null;
    }
}