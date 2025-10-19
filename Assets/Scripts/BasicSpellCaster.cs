using UnityEngine;
using UnityEngine.InputSystem;

public class BasicSpellCaster : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MouseAimerTopDown aimer;     // Компонент наведения на мышь на игроке
    [SerializeField] private Transform muzzle;            // Точка выстрела (дочерний объект у Player)
    [SerializeField] private GameObject projectilePrefab; // Префаб с компонентом Projectile

    [Header("Tuning")]
    [SerializeField] private float fireCooldown = 0.22f;  // КД между выстрелами

    private float _cooldown;
    private ParticleSystem _muzzleFlash;

    
    private void Awake()
    {
        if (muzzle != null) muzzle.TryGetComponent(out _muzzleFlash);
    }

    private void Update()
    {
        if (_cooldown > 0f) _cooldown -= Time.deltaTime;
    }

    // Input System (Player Input → Behavior: Send Messages) → Action: E1Tap (Mouse Left Button)
    public void OnE1Tap(InputValue value)
    {
        Debug.Log("E1Tap pressed");
        if (!value.isPressed) return;
        TryShoot();
    }


    private void TryShoot()
   
    {
        if (_cooldown > 0f) return;
        if (aimer == null || muzzle == null || projectilePrefab == null) return;

        Vector3 dir = aimer.AimDirection.sqrMagnitude > 0.0001f
            ? aimer.AimDirection
            : transform.forward;

        var go = Instantiate(projectilePrefab, muzzle.position, Quaternion.identity);
        if (!go.TryGetComponent<Projectile>(out var proj))
        {
            Destroy(go);
            return;
        }

        proj.Fire(transform, dir);

        if (_muzzleFlash != null) _muzzleFlash.Play();

        _cooldown = fireCooldown;
    }