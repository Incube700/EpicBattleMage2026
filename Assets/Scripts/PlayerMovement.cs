using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float acceleration = 20f;
    [SerializeField] private float deceleration = 30f;

    private Rigidbody _rb;
    private Vector2 _moveInput;
    private Vector3 _currentVelocity;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }

    private void FixedUpdate()
    {
        Vector3 targetVelocity = new Vector3(_moveInput.x, 0f, _moveInput.y) * moveSpeed;

        float rate = targetVelocity.sqrMagnitude >= _currentVelocity.sqrMagnitude ? acceleration : deceleration;
        _currentVelocity = Vector3.MoveTowards(_currentVelocity, targetVelocity, rate * Time.fixedDeltaTime);

        _rb.MovePosition(_rb.position + _currentVelocity * Time.fixedDeltaTime);
    }
}
