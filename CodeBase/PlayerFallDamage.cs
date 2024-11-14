using Mirror;
using UnityEngine;

public class PlayerFallDamage : NetworkBehaviour
{
    [SerializeField] private float minFallHeight = 5f;     // Минимальная высота падения для получения урона
    [SerializeField] private float maxFallHeight = 20f;    // Максимальная высота падения для максимального урона
    [SerializeField] private int maxFallDamage = 50;       // Максимальный урон от падения

    private PlayerHealth _playerHealth;
    private bool _isFalling;
    private float _fallStartHeight;

    private void Start()
    {
        _playerHealth = GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        if (!isServer) return; 

        CheckFallDamage();
    }

    private void CheckFallDamage()
    {
        if (!_isFalling && !IsGrounded())
        {
            _isFalling = true;
            _fallStartHeight = transform.position.y;
        }
        
        if (_isFalling && IsGrounded())
        {
            _isFalling = false;
            float fallDistance = _fallStartHeight - transform.position.y;

            if (fallDistance >= minFallHeight)
            {
                ApplyFallDamage(fallDistance);
            }
        }
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }

    [Server]
    private void ApplyFallDamage(float fallDistance)
    {
        float damageFactor = Mathf.InverseLerp(minFallHeight, maxFallHeight, fallDistance);
        int damage = Mathf.RoundToInt(damageFactor * maxFallDamage);

        _playerHealth.TakeDamage(damage);
    }
}