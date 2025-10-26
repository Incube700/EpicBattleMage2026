using UnityEngine;

namespace EpicMageBattle.Combat
{
    public interface IDamageable
    {
        void ApplyDamage(float amount, Element element, GameObject source, Vector3 hitPoint);
    }
}