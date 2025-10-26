using UnityEngine;

public class SpellPickup : MonoBehaviour
{
    [SerializeField] private SpellConfig _spell;
    [SerializeField] private string _playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(_playerTag)) return;
        var book = other.GetComponent<PlayerSpellbook>();
        if (book == null) return;

        if (book.AddSpell(_spell))
        {
            Destroy(gameObject);
        }
    }
}