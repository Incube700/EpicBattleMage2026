using UnityEngine;

[DefaultExecutionOrder(-5)]
public class PlayerAim : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private float _turnSpeed = 720f;

    public Vector3 AimPoint { get; private set; }
    public Vector3 AimDirection { get; private set; }

    private void Awake()
    {
        if (_camera == null) _camera = Camera.main;
    }

    private void Update()
    {
        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, 1000f, _groundMask, QueryTriggerInteraction.Ignore))
        {
            AimPoint = hit.point;
            var dir = AimPoint - transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.001f)
            {
                AimDirection = dir.normalized;
                var targetRot = Quaternion.LookRotation(AimDirection, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, _turnSpeed * Time.deltaTime);
            }
        }
        else
        {
            AimDirection = transform.forward;
        }
    }
}