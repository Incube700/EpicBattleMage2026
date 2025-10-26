using UnityEngine;

[RequireComponent(typeof(Collider))]
public class KillVolume : MonoBehaviour
{
    [SerializeField] private float _killDamage = 99999f;
    [SerializeField] private bool _onlyAffectTag = false;
    [SerializeField] private string _targetTag = "Player";
    [SerializeField] private LayerMask _affectsLayers = ~0;

    private void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & _affectsLayers) == 0) return;
        if (_onlyAffectTag && !other.CompareTag(_targetTag)) return;

        var hp = other.GetComponent<SimpleHealth>();
        if (hp == null) return;

        hp.TakeDamage(_killDamage);
    }
}