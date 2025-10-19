using UnityEngine;
using UnityEngine.Serialization;

public class Health : MonoBehaviour
{
    [FormerlySerializedAs("maxHP")] [SerializeField] private float maxHp = 100f;
    private float _hp;

    private void Awake() => _hp = maxHp;

    public void ApplyDamage(float amount, Vector3 hitPoint, Vector3 impulse)
    {
        _hp -= amount;
        if (_hp <= 0f)
        {
            gameObject.SetActive(false);
            return;
        }

        if (TryGetComponent<Rigidbody>(out var rb))
            rb.AddForce(impulse, ForceMode.Impulse);
    }
}

