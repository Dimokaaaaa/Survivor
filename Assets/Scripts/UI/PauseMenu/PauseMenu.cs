using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Minecraft2D.Player;
using Minecraft2D.Saving;

namespace Minecraft2D.UI
{
    public class PauseMenu : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private GameObject panel;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button saveButton;
        [SerializeField] private Button exitToMenuButton;

        private bool isPaused;
        private GameObject player;

        private void Start()
        {
            panel.SetActive(false);
            player = GameObject.FindGameObjectWithTag("Player");

            resumeButton.onClick.AddListener(Resume);
            saveButton.onClick.AddListener(SaveGame);
            exitToMenuButton.onClick.AddListener(ExitToMenu);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isPaused) Resume();
                else Pause();
            }
        }

        private void Pause()
        {
            isPaused = true;
            panel.SetActive(true);
            Time.timeScale = 0f;
            SetPlayerScripts(false);
        }

        private void Resume()
        {
            isPaused = false;
            panel.SetActive(false);
            Time.timeScale = 1f;
            SetPlayerScripts(true);
        }

        private void SetPlayerScripts(bool enabled)
        {
            if (player == null) return;

            if (player.TryGetComponent<PlayerMovement>(out var m)) m.enabled = enabled;
            if (player.TryGetComponent<PlayerPlacer>(out var p)) p.enabled = enabled;
            if (player.TryGetComponent<PlayerDigger>(out var d)) d.enabled = enabled;
            if (player.TryGetComponent<PlayerAnimator>(out var a)) a.enabled = enabled;
        }

        private void SaveGame()
        {
            SaveSystem.Instance?.Save();
        }

        private void ExitToMenu()
        {
            Time.timeScale = 1f;
            SaveSystem.Instance?.Save();
            SceneManager.LoadScene("MainMenu");
        }
    }
}