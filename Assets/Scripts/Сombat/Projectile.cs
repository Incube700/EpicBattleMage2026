using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float _gravityMultiplier = 0f;
    [SerializeField] private LayerMask _hitMask = ~0;
    [SerializeField] private float _radius = 0.1f;

    private float _damage;
    private float _life;
    private float _dieAt;
    private GameObject _owner;
    private bool _launched;

    private void Reset()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    public void Launch(Vector3 velocity, float damage, float lifeTime, GameObject owner)
    {
        if (_rb == null) _rb = GetComponent<Rigidbody>();
        _owner = owner;
        _damage = damage;
        _life = Mathf.Max(0.1f, lifeTime);
        _dieAt = Time.time + _life;
        _launched = true;

        _rb.velocity = velocity;
    }

    private void FixedUpdate()
    {
        if (!_launched) return;

        if (_gravityMultiplier > 0f)
            _rb.AddForce(Physics.gravity * _gravityMultiplier, ForceMode.Acceleration);

        if (Time.time >= _dieAt)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_launched) return;
        if (_owner != null && other.attachedRigidbody != null && other.attachedRigidbody.gameObject == _owner) return;
        if (((1 << other.gameObject.layer) & _hitMask) == 0) return;

        var hp = other.GetComponent<SimpleHealth>();
        if (hp != null)
        {
            hp.TakeDamage(_damage);
        }

        Destroy(gameObject);
    }
}