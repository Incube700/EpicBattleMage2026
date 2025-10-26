using UnityEngine;

[CreateAssetMenu(menuName = "EpicMageBattle/Spell Config", fileName = "SpellConfig")]
public class SpellConfig : ScriptableObject
{
    [Header("Base")]
    public string SpellId = "fire_basic";
    public SpellSchool School = SpellSchool.Fire;
    [Min(0f)] public float Damage = 10f;
    [Min(0.1f)] public float Cooldown = 0.35f;

    [Header("Projectile")]
    public GameObject ProjectilePrefab;
    [Min(0.1f)] public float ProjectileSpeed = 18f;
    [Min(0.1f)] public float ProjectileLifeTime = 5f;
    public float SpreadDeg = 0f;

    [Header("VFX/SFX (опционально)")]
    public GameObject MuzzleVFX;
}