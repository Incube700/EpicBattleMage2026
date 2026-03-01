using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference _move;   // Vector2
    [SerializeField] private InputActionReference _dash;   // Button

    [Header("Camera-relative")]
    [SerializeField] private Camera _camera; // если пусто, возьмёт Camera.main

    [Header("Movement")]
    [SerializeField] private float _maxSpeed = 6f;
    [SerializeField] private float _acceleration = 35f;
    [SerializeField] private float _deceleration = 25f;
    [SerializeField] private float _inputDeadZone = 0.08f;

    [Header("Dash")]
    [SerializeField] private float _dashDistance = 6f;
    [SerializeField] private float _dashDuration = 0.15f;
    [SerializeField] private float _dashCooldown = 0.6f;

    [Header("Misc")]
    [SerializeField] private float _yLock = 0f; // высота арены
    [SerializeField] private bool _clampY = true;

    private Rigidbody _rb;
    private Vector2 _moveAxis;          // сырое
    private Vector2 _moveAxisSmooth;    // сглаженное
    private Vector2 _moveAxisVel;       // для SmoothDamp
    private bool _isDashing;
    private float _nextDashTime;

    public bool IsDashing => _isDashing;
    public Vector3 VelocityPlanar => new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        if (_camera == null) _camera = Camera.main;
    }

    private void OnEnable()
    {
        _move?.action.Enable();
        _dash?.action.Enable();

        if (_dash != null) _dash.action.performed += OnDash;
    }

    private void OnDisable()
    {
        if (_dash != null) _dash.action.performed -= OnDash;
    }

    private void Update()
    {
        // читаем ввода в Update
        _moveAxis = _move != null ? _move.action.ReadValue<Vector2>() : Vector2.zero;

        // сглаживание, чтобы не дёргалось от «квадратного» WASD
        _moveAxisSmooth = Vector2.SmoothDamp(_moveAxisSmooth, _moveAxis, ref _moveAxisVel, 0.05f);
        if (_moveAxisSmooth.magnitude < _inputDeadZone) _moveAxisSmooth = Vector2.zero;
    }

    private void FixedUpdate()
    {
        if (_isDashing)
        {
            // во время дэша скоростью рулит корутина
            HardClampY();
            return;
        }

        // камера-относительное направление на плоскости
        Vector3 forward = Vector3.forward;
        Vector3 right = Vector3.right;

        if (_camera != null)
        {
            forward = _camera.transform.forward; forward.y = 0f; forward.Normalize();
            right   = _camera.transform.right;   right.y   = 0f; right.Normalize();
        }

        Vector3 inputDir = (right * _moveAxisSmooth.x + forward * _moveAxisSmooth.y);
        if (inputDir.sqrMagnitude > 1e-4f) inputDir.Normalize();

        Vector3 v = VelocityPlanar;
        Vector3 targetV = inputDir * _maxSpeed;

        float accel = inputDir.sqrMagnitude > 1e-4f ? _acceleration : _deceleration;
        v = Vector3.MoveTowards(v, targetV, accel * Time.fixedDeltaTime);

        _rb.velocity = new Vector3(v.x, 0f, v.z);

        HardClampY();
    }

    private void OnDash(InputAction.CallbackContext _)
    {
        if (Time.time < _nextDashTime || _isDashing) return;

        // направление дэша: по входу, иначе по взгляду/вперёд
        Vector3 dir;
        if (_moveAxisSmooth.sqrMagnitude > 1e-4f)
        {
            Vector3 forward = _camera != null ? _camera.transform.forward : Vector3.forward;
            Vector3 right   = _camera != null ? _camera.transform.right   : Vector3.right;
            forward.y = 0f; right.y = 0f; forward.Normalize(); right.Normalize();
            dir = (right * _moveAxisSmooth.x + forward * _moveAxisSmooth.y).normalized;
        }
        else
        {
            dir = transform.forward.sqrMagnitude > 0.5f ? transform.forward : Vector3.forward;
            dir.y = 0f; dir.Normalize();
        }

        StartCoroutine(DashRoutine(dir));
        _nextDashTime = Time.time + _dashCooldown;
    }

    private IEnumerator DashRoutine(Vector3 dir)
    {
        _isDashing = true;

        float speed = _dashDistance / Mathf.Max(0.05f, _dashDuration);
        float t = 0f;

        // временно «отключим» обычное ускорение: задаём быструю скорость напрямую
        while (t < _dashDuration)
        {
            _rb.velocity = dir * speed;
            HardClampY();
            t += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        // мягко гасим до обычного движения
        _rb.velocity = dir * Mathf.Min(_maxSpeed, speed * 0.25f);
        _isDashing = false;
    }

    private void HardClampY()
    {
        if (!_clampY) return;
        var pos = _rb.position;
        pos.y = _yLock;
        _rb.position = pos;
    }
}
