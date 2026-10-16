using UnityEngine;
using UnityEngine.UI;

namespace Minecraft2D.UI.MainMenu
{
    /// <summary>
    /// Панель настроек. Пока только громкость.
    /// </summary>
    public class SettingsPanel : MonoBehaviour
    {
        private const string VolumePrefsKey = "MasterVolume";

        [SerializeField] private Slider volumeSlider;
        [SerializeField] private Button closeButton;

        private void Start()
        {
            float savedVolume = PlayerPrefs.GetFloat(VolumePrefsKey, 0.8f);
            volumeSlider.value = savedVolume;
            AudioListener.volume = savedVolume;

            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
            closeButton.onClick.AddListener(Close);
        }

        private void OnVolumeChanged(float value)
        {
            AudioListener.volume = value;
            PlayerPrefs.SetFloat(VolumePrefsKey, value);
        }

        private void Close()
        {
            PlayerPrefs.Save();
            gameObject.SetActive(false);
        }
    }
}