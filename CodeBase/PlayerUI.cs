using CodeBase.Scripts;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : NetworkBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private TextMeshProUGUI playerNameText;
    private CustomNetworkRoomPlayer _customNetworkRoomPlayer;

    public void Construct(string playerName, CustomNetworkRoomPlayer customNetworkRoomPlayer)
    {
        playerNameText.text = playerName;

        if (playerHealth != null && healthSlider != null)
        {
            playerHealth.OnHealthUpdated += UpdateHealthUI;
            _customNetworkRoomPlayer = customNetworkRoomPlayer;
            _customNetworkRoomPlayer.OnPlayerNameChange += UpdateNameUI;
            healthSlider.maxValue = 100;
            healthSlider.value = 100;
        }
        else
        {
            Debug.LogWarning("HealthUI: Не удалось найти PlayerHealth или Slider.");
        }
    }

    [ClientRpc]
    private void UpdateNameUI(string playerName)
    {
        playerNameText.text = playerName;
    }

    [ClientRpc]
    private void UpdateHealthUI(int currentHealth, int maxHealth)
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthUpdated -= UpdateHealthUI;
            if (_customNetworkRoomPlayer != null)
            {
                _customNetworkRoomPlayer.OnPlayerNameChange -= UpdateNameUI;
            }
        }
    }
}