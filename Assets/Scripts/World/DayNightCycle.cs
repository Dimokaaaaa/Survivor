using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Minecraft2D.World
{
    /// <summary>
    /// Цикл дня и ночи. Каждые N секунд — смена.
    /// Ночь: затемнение + больше зомби.
    /// </summary>
    public class DayNightCycle : MonoBehaviour
    {
        public static DayNightCycle Instance { get; private set; }

        [Header("Тайминги")]
        [SerializeField] private float dayDuration = 300f;   // 5 минут
        [SerializeField] private float nightDuration = 300f; // 5 минут
        [SerializeField] private float transitionDuration = 3f; // плавный переход

        [Header("Освещение")]
        [SerializeField] private Image darknessOverlay;      // UI Image на весь экран
        [SerializeField] private Color dayColor = new Color(0f, 0f, 0f, 0f);
        [SerializeField] private Color nightColor = new Color(0f, 0f, 0.15f, 0.7f);

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI timerText;

        public event Action<bool> OnDayNightChanged; // true = день, false = ночь
        public bool IsDay { get; private set; } = true;

        private float gameTime;
        private float phaseTimer;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void Start()
        {
            IsDay = true;
            phaseTimer = dayDuration;
            OnDayNightChanged?.Invoke(true);
        }

        private void Update()
        {
            gameTime += Time.deltaTime;
            phaseTimer -= Time.deltaTime;

            UpdateTimerText();
            UpdateDarkness();

            if (phaseTimer <= 0f)
                SwitchPhase();
        }

        private void SwitchPhase()
        {
            IsDay = !IsDay;
            phaseTimer = IsDay ? dayDuration : nightDuration;
            OnDayNightChanged?.Invoke(IsDay);
            Debug.Log(IsDay ? "Наступил день" : "Наступила ночь");
        }

        private void UpdateTimerText()
        {
            if (timerText == null) return;

            int minutes = Mathf.FloorToInt(gameTime / 60f);
            int seconds = Mathf.FloorToInt(gameTime % 60f);
            string phase = IsDay ? "День" : "Ночь";
            timerText.text = $"{phase}  |  {minutes:00}:{seconds:00}";
        }

        private void UpdateDarkness()
        {
            if (darknessOverlay == null) return;

            // Плавный переход в конце фазы
            float targetAlpha;
            if (IsDay)
                targetAlpha = 0f;
            else
                targetAlpha = nightColor.a;

            Color c = darknessOverlay.color;
            c = Color.Lerp(c, new Color(nightColor.r, nightColor.g, nightColor.b, targetAlpha), Time.deltaTime / transitionDuration);
            darknessOverlay.color = c;
        }
    }
}