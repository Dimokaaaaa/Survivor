using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Minecraft2D.UI.MainMenu
{
    /// <summary>
    /// Управляет главным меню: связывает кнопки с действиями.
    /// Игровой логики тут нет — только навигация по меню.
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        [Header("Кнопки")]
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button exitButton;

        [Header("Панели")]
        [SerializeField] private GameObject settingsPanel;

        private void Start()
        {
            // Прячем панель настроек при старте
            if (settingsPanel != null)
                settingsPanel.SetActive(false);

            // Подписываемся на клики
            newGameButton.onClick.AddListener(OnNewGameClicked);
            continueButton.onClick.AddListener(OnContinueClicked);
            settingsButton.onClick.AddListener(OnSettingsClicked);
            exitButton.onClick.AddListener(OnExitClicked);

            // «Продолжить» доступна только если есть сохранение
            continueButton.interactable = SaveExists();
        }

        /// <summary>
        /// Заглушка. На этапе 8 заменим на реальную проверку файла сохранения.
        /// </summary>
        private bool SaveExists()
        {
            return false;
        }

        private void OnNewGameClicked()
        {
            SceneManager.LoadScene("Game");
        }

        private void OnContinueClicked()
        {
            SceneManager.LoadScene("Game");
        }

        private void OnSettingsClicked()
        {
            settingsPanel.SetActive(true);
        }

        private void OnExitClicked()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}