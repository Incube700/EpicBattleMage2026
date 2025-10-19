using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 30f;
    [SerializeField] private float damage = 20f;
    [SerializeField] private float lifetime = 2.5f;

    private Rigidbody _rb;
    private Transform _owner;

    public void Fire(Transform owner, Vector3 dir)
    {
        _owner = owner;
        if (_rb == null) _rb = GetComponent<Rigidbody>();
        _rb.velocity = dir.normalized * speed;
        Invoke(nameof(Kill), lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == _owner) return;

        if (other.TryGetComponent(out Health hp))
            hp.ApplyDamage(damage, transform.position, _rb.velocity.normalized * 2f);

        Kill();
    }

    private void Kill()
    {
        Destroy(gameObject);
    }
}