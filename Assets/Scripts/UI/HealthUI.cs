using UnityEngine;
using TMPro;
using Minecraft2D.Player;

namespace Minecraft2D.UI
{
    public class HealthUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI healthText;

        private void Start()
        {
            if (PlayerHealth.Instance != null)
                PlayerHealth.Instance.OnHealthChanged += OnHealthChanged;

            Refresh();
        }

        private void OnDestroy()
        {
            if (PlayerHealth.Instance != null)
                PlayerHealth.Instance.OnHealthChanged -= OnHealthChanged;
        }

        private void OnHealthChanged(int current, int max)
        {
            Refresh();
        }

        private void Refresh()
        {
            if (PlayerHealth.Instance == null || healthText == null) return;
            healthText.text = $"HP: {PlayerHealth.Instance.CurrentHealth} / {PlayerHealth.Instance.MaxHealth}";
        }
    }
}