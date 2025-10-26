using UnityEngine;

public class SimpleHealth : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private bool _destroyOnDeath = true;

    public float MaxHealth => _maxHealth;
    public float Current { get; private set; }

    private void Awake()
    {
        Current = _maxHealth;
    }

    public void TakeDamage(float dmg)
    {
        if (dmg <= 0f) return;
        Current -= dmg;
        if (Current <= 0f)
        {
            if (_destroyOnDeath) Destroy(gameObject);
        }
    }
}