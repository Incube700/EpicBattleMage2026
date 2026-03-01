using UnityEngine;

public class PlayerSpellbook : MonoBehaviour
{
    [SerializeField] private SpellConfig[] _slots = new SpellConfig[3];

    public int SlotsCount => _slots.Length;
    public SpellConfig GetSlot(int index) => index >= 0 && index < _slots.Length ? _slots[index] : null;

    public bool AddSpell(SpellConfig config)
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i] == null)
            {
                _slots[i] = config;
                return true;
            }
        }
        return false;
    }

    public int GetFilledCount()
    {
        int c = 0;
        for (int i = 0; i < _slots.Length; i++)
            if (_slots[i] != null) c++;
        return c;
    }

    public SpellConfig[] ConsumeAll()
    {
        var filled = GetFilledCount();
        if (filled == 0) return new SpellConfig[0];
        var result = new SpellConfig[filled];
        int w = 0;
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i] != null)
            {
                result[w++] = _slots[i];
                _slots[i] = null;
            }
        }
        return result;
    }
}