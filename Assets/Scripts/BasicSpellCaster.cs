using UnityEngine;
using UnityEngine.InputSystem;

public class BasicSpellCaster : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MouseAimerTopDown aimer;
    [SerializeField] private Transform muzzle;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private ParticleSystem muzzleFlash;   // укажи PS тут (может быть дочерним у Muzzle)

    [Header("Tuning")]
    [SerializeField] private float fireCooldown = 0.22f;

    private float _cooldown;

    private void Awake()
    {
        if (muzzleFlash == null && muzzle != null)
            muzzleFlash = muzzle.GetComponentInChildren<ParticleSystem>(true);
    }

    private void Update()
    {
        if (_cooldown > 0f) _cooldown -= Time.deltaTime;
    }

    // Input System → Send Messages → Action: E1Tap
    public void OnE1Tap(InputValue value)
    {
        if (!value.isPressed) return;
        TryShoot();
    }

    private void TryShoot()
    {
        if (_cooldown > 0f || aimer == null || muzzle == null || projectilePrefab == null) return;

        var go = Instantiate(projectilePrefab, muzzle.position, Quaternion.identity);
        var proj = go.GetComponent<Projectile>();
        if (proj == null) { Debug.LogError("Prefab has no Projectile"); Destroy(go); return; }

        var dir = aimer.AimDirection.sqrMagnitude > 0.0001f ? aimer.AimDirection : transform.forward;
        proj.Fire(transform, dir);

        if (muzzleFlash != null) muzzleFlash.Play(true);

        _cooldown = fireCooldown;
    }
}