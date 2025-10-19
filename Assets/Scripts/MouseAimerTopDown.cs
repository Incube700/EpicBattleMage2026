using UnityEngine;
using UnityEngine.InputSystem;

public class MouseAimerTopDown : MonoBehaviour
{
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float rotateSpeed = 720f;

    public Vector3 AimPoint { get; private set; }
    public Vector3 AimDirection =>
        (new Vector3(AimPoint.x, transform.position.y, AimPoint.z) - transform.position).normalized;

    private Camera _cam;

    private void Awake()
    {
        _cam = Camera.main;
    }

    private void Update()
    {
        if (_cam == null) return;

        Vector2 mouse = Mouse.current != null ? Mouse.current.position.ReadValue() : (Vector2)Input.mousePosition;
        Ray ray = _cam.ScreenPointToRay(mouse);

        if (Physics.Raycast(ray, out var hit, 200f, groundMask))
        {
            AimPoint = hit.point;
            Vector3 lookPos = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            Vector3 dir = lookPos - transform.position;
            if (dir.sqrMagnitude > 0.0001f)
            {
                Quaternion target = Quaternion.LookRotation(dir, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, target, rotateSpeed * Time.deltaTime);
            }
        }
    }
}