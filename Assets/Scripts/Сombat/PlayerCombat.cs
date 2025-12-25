using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerSpellbook))]
public class PlayerCombat : MonoBehaviour
{
    [Header("Input System")]
    [SerializeField] private InputActionReference _firePrimary;   // ЛКМ
    [SerializeField] private InputActionReference _fireSecondary; // ПКМ
    [SerializeField] private InputActionReference _ultimate;      // Q (или что назначишь)

    [Header("Casting")]
    [SerializeField] private Transform _muzzle;
    [SerializeField] private PlayerAim _aim;
    [SerializeField] private float _recoilSpreadOnMove = 2f;

    [Header("Ultimate")]
    [SerializeField] private GameObject _arcaneCirclePrefab;

    private PlayerSpellbook _book;
    private readonly Dictionary<SpellConfig, float> _cooldowns = new();

    private void Awake()
    {
        _book = GetComponent<PlayerSpellbook>();
        if (_aim == null) _aim = GetComponent<PlayerAim>();
        if (_muzzle == null) _muzzle = transform;
    }

    private void OnEnable()
    {
        _firePrimary?.action.Enable();
        _fireSecondary?.action.Enable();
        _ultimate?.action.Enable();

        if (_firePrimary != null)   _firePrimary.action.performed += OnFirePrimary;
        if (_fireSecondary != null) _fireSecondary.action.performed += OnFireSecondary;
        if (_ultimate != null)      _ultimate.action.performed += OnUltimate;
    }

    private void OnDisable()
    {
        if (_firePrimary != null)   _firePrimary.action.performed -= OnFirePrimary;
        if (_fireSecondary != null) _fireSecondary.action.performed -= OnFireSecondary;
        if (_ultimate != null)      _ultimate.action.performed -= OnUltimate;
    }

    private void OnFirePrimary(InputAction.CallbackContext _)
    {
        CastSlot(0);
    }

    private void OnFireSecondary(InputAction.CallbackContext _)
    {
        var hasSecond = _book.GetSlot(1) != null;
        CastSlot(hasSecond ? 1 : 0);
    }

    private void OnUltimate(InputAction.CallbackContext _)
    {
        var consumed = _book.ConsumeAll();
        if (consumed.Length == 0) return;

        var pos = _aim != null ? _aim.AimPoint : transform.position;
        var go = Instantiate(_arcaneCirclePrefab, pos, Quaternion.identity);
        var circle = go.GetComponent<ArcaneCircleUltimate>();

        float totalDamage = 0f;
        foreach (var s in consumed) totalDamage += s != null ? s.Damage : 0f;
        float radius = Mathf.Lerp(3f, 6.5f, (consumed.Length - 1) / 2f);
        float dps = Mathf.Max(10f, totalDamage);

        circle.Configure(radius, dps, 3.5f);
    }

    private void CastSlot(int index)
    {
        var spell = _book.GetSlot(index);
        if (spell == null || spell.ProjectilePrefab == null) return;

        if (_cooldowns.TryGetValue(spell, out var readyAt) && Time.time < readyAt) return;

        var dir = _aim != null && _aim.AimDirection.sqrMagnitude > 0.001f ? _aim.AimDirection : transform.forward;

        float spread = spell.SpreadDeg;
        if (_recoilSpreadOnMove > 0f && GetComponent<Rigidbody>() != null)
        {
            if (GetComponent<Rigidbody>().velocity.sqrMagnitude > 0.1f) spread += _recoilSpreadOnMove;
        }

        if (spread > 0f)
        {
            var yaw = Random.Range(-spread * 0.5f, spread * 0.5f);
            dir = Quaternion.Euler(0f, yaw, 0f) * dir;
        }

        if (spell.MuzzleVFX != null)
            Instantiate(spell.MuzzleVFX, _muzzle.position, Quaternion.LookRotation(dir));

        var proj = Instantiate(spell.ProjectilePrefab, _muzzle.position, Quaternion.LookRotation(dir));
        var p = proj.GetComponent<Projectile>();
        if (p != null)
        {
            p.Launch(dir * spell.ProjectileSpeed, spell.Damage, spell.ProjectileLifeTime, gameObject);
        }

        _cooldowns[spell] = Time.time + spell.Cooldown;
    }
}
