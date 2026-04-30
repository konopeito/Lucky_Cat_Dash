using UnityEngine;

[ExecuteAlways]
public class CameraClamp2D : MonoBehaviour
{
    [Header("Target (optional)")]
    [SerializeField] private Transform target;

    [Header("Follow")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 1f, -10f);
    [SerializeField] private float smoothTime = 0.12f;

    [Header("World Bounds (camera will never show outside these)")]
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;
    [SerializeField] private float minY = -5f;
    [SerializeField] private float maxY = 5f;

    private Camera cam;
    private Vector3 velocity;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam == null) cam = Camera.main;
    }

    private void LateUpdate()
    {
        if (cam == null) return;

      
        Vector3 desired = transform.position;
        if (target != null)
            desired = target.position + offset;

        // Clamp so the camera view never goes outside bounds
        Vector3 clamped = ClampToBounds(desired);

        //  Smooth move
        transform.position = Application.isPlaying
            ? Vector3.SmoothDamp(transform.position, clamped, ref velocity, smoothTime)
            : clamped;
    }

    private Vector3 ClampToBounds(Vector3 desiredPos)
    {
        
        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;

        float clampedX = Mathf.Clamp(desiredPos.x, minX + halfWidth, maxX - halfWidth);
        float clampedY = Mathf.Clamp(desiredPos.y, minY + halfHeight, maxY - halfHeight);

        return new Vector3(clampedX, clampedY, desiredPos.z);
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize bounds in scene view
        Gizmos.color = Color.cyan;
        Vector3 center = new Vector3((minX + maxX) * 0.5f, (minY + maxY) * 0.5f, 0f);
        Vector3 size = new Vector3((maxX - minX), (maxY - minY), 0f);
        Gizmos.DrawWireCube(center, size);
    }
}
