using UnityEngine;

namespace Minecraft2D.UI
{
    /// <summary>
    /// Шкала прогресса над объектом в мировых координатах.
    /// Заполнение от 0 до 1.
    /// </summary>
    public class WorldProgressBar : MonoBehaviour
    {
        [SerializeField] private Transform fillTransform;
        [SerializeField] private float maxWidth = 1f;

        private SpriteRenderer[] renderers;

        private void Awake()
        {
            renderers = GetComponentsInChildren<SpriteRenderer>();
            Hide();
        }

        /// <summary>
        /// Показать шкалу в позиции над объектом.
        /// </summary>
        public void Show(Vector3 worldPosition)
        {
            transform.position = worldPosition;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Установить заполнение (0..1).
        /// </summary>
        public void SetProgress(float value01)
        {
            float width = Mathf.Clamp01(value01) * maxWidth;
            Vector3 scale = fillTransform.localScale;
            scale.x = width;
            fillTransform.localScale = scale;

            // Сдвигаем fill так, чтобы он "рос" слева направо
            Vector3 pos = fillTransform.localPosition;
            pos.x = -(maxWidth - width) * 0.5f;
            fillTransform.localPosition = pos;
        }
    }
}