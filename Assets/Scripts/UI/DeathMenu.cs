using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Minecraft2D.Player;
using Minecraft2D.Saving;

namespace Minecraft2D.UI
{
    public class DeathMenu : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private GameObject panel;
        [SerializeField] private Button respawnButton;
        [SerializeField] private Button saveAndExitButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private GameObject settingsPanel;

        [Header("Скрипты игрока — отключить при смерти")]
        [SerializeField] private MonoBehaviour[] playerScriptsToDisable;

        [Header("Другие объекты — отключить")]
        [SerializeField] private GameObject[] objectsToDisable;

        private void Start()
        {
            panel.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(false);

            respawnButton.onClick.AddListener(OnRespawn);
            saveAndExitButton.onClick.AddListener(OnSaveAndExit);
            settingsButton.onClick.AddListener(OnSettings);

            if (PlayerHealth.Instance != null)
                PlayerHealth.Instance.OnPlayerDied += Show;
        }

        private void OnDestroy()
        {
            if (PlayerHealth.Instance != null)
                PlayerHealth.Instance.OnPlayerDied -= Show;
        }

        private void Show()
        {
            panel.SetActive(true);

            // Замораживаем время
            Time.timeScale = 0f;

            // Отключаем скрипты игрока
            foreach (var script in playerScriptsToDisable)
                if (script != null) script.enabled = false;

            // Отключаем другие объекты (например, спавнер зомби)
            foreach (var obj in objectsToDisable)
                if (obj != null) obj.SetActive(false);
        }

        private void OnRespawn()
        {
            panel.SetActive(false);
            Time.timeScale = 1f;

            foreach (var script in playerScriptsToDisable)
                if (script != null) script.enabled = true;

            foreach (var obj in objectsToDisable)
                if (obj != null) obj.SetActive(true);

            PlayerHealth.Instance?.Respawn();
        }

        private void OnSaveAndExit()
        {
            Time.timeScale = 1f;   // ← вернуть перед выходом
            SaveSystem.Instance?.Save();
            SceneManager.LoadScene("MainMenu");
        }

        private void OnSettings()
        {
            settingsPanel.SetActive(!settingsPanel.activeSelf);
        }
    }
}