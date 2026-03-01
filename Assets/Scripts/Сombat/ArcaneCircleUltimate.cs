using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class ArcaneCircleUltimate : MonoBehaviour
{
    [SerializeField] private SphereCollider _col;
    [SerializeField] private float _tickInterval = 0.25f;

    private float _radius;
    private float _dps;
    private float _duration;

    private void Awake()
    {
        if (_col == null) _col = GetComponent<SphereCollider>();
        _col.isTrigger = true;
    }

    public void Configure(float radius, float dps, float duration)
    {
        _radius = Mathf.Max(0.5f, radius);
        _dps = Mathf.Max(1f, dps);
        _duration = Mathf.Max(0.5f, duration);

        transform.localScale = Vector3.one * (_radius * 2f);
        _col.radius = 0.5f; // потому что масштабим объект

        StartCoroutine(DamageLoop());
    }

    private IEnumerator DamageLoop()
    {
        float elapsed = 0f;
        var list = new Collider[32];

        while (elapsed < _duration)
        {
            int count = Physics.OverlapSphereNonAlloc(transform.position, _radius, list, ~0, QueryTriggerInteraction.Ignore);
            float dmg = _dps * _tickInterval;

            for (int i = 0; i < count; i++)
            {
                var hp = list[i].GetComponent<SimpleHealth>();
                if (hp != null) hp.TakeDamage(dmg);
            }

            elapsed += _tickInterval;
            yield return new WaitForSeconds(_tickInterval);
        }

        Destroy(gameObject);
    }
}