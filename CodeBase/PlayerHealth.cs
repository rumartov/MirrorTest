using System;
using Mirror;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    public bool IsDead => _currentHealth <= 0;

    public event Action<int, int> OnHealthUpdated;

    private int _maxHealth = 100;

    [SyncVar(hook = nameof(OnHealthChanged))]
    private int _currentHealth;

    public void Construct(int maxHealth) => _maxHealth = maxHealth;

    public override void OnStartServer()
    {
        base.OnStartServer();
        _currentHealth = _maxHealth;
    }

    [Server]
    public void TakeDamage(int amount)
    {
        if (IsDead) return;

        _currentHealth = Mathf.Max(_currentHealth - amount, 0);
        
        if (IsDead)
        {
            RpcHandleDeath();
            Respawn(gameObject);
        }
        
        OnHealthUpdated?.Invoke(_currentHealth, _maxHealth);
    }

    [Server]
    public void Heal(int amount)
    {
        if (IsDead) return;

        _currentHealth = Mathf.Min(_currentHealth + amount, _maxHealth);
        OnHealthUpdated?.Invoke(_currentHealth, _maxHealth);
    }
    
    private void OnHealthChanged(int oldHealth, int newHealth)
    {
        OnHealthUpdated?.Invoke(newHealth, _maxHealth);
    }

    [ClientRpc]
    private void RpcHandleDeath()
    {
        if (isLocalPlayer)
            Debug.Log("You are dead!");
        else
            Debug.Log("Player has died.");
    }

    [Server]
    public void ResetHealth() => _currentHealth = _maxHealth;

    [ClientRpc]
    private void Respawn(GameObject player)
    {
        player.SetActive(false);
        player.transform.position = new Vector3(0, 2, 0);
        player.SetActive(true);

        player.GetComponent<PlayerHealth>()?.ResetHealth();
    }
}