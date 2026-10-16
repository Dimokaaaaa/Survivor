using UnityEngine;
using UnityEngine.UI;

namespace Minecraft2D.UI
{
    /// <summary>
    /// Миникарта через Texture2D. Обновляется раз в N секунд.
    /// Показывает область вокруг игрока в 2 раза больше видимой.
    /// </summary>
    public class MinimapController : MonoBehaviour
    {
        [Header("Ссылки")]
        [SerializeField] private Transform player;
        [SerializeField] private RawImage minimapImage;

        [Header("Покрытие (в тайлах)")]
        [SerializeField] private int visibleTilesX = 40;
        [SerializeField] private int visibleTilesY = 40;
        [SerializeField] private int pixelsPerTile = 5;

        [Header("Обновление")]
        [SerializeField] private float updateInterval = 0.5f;

        [Header("Цвета")]
        [SerializeField] private Color background = new Color(0.35f, 0.55f, 0.30f);
        [SerializeField] private Color playerColor = Color.white;
        [SerializeField] private Color treeColor = new Color(0.25f, 0.55f, 0.15f);
        [SerializeField] private Color rockColor = new Color(0.55f, 0.55f, 0.55f);
        [SerializeField] private Color zombieColor = new Color(0.75f, 0.85f, 0.20f);

        private Texture2D texture;
        private Color[] pixels;
        private int textureWidth;
        private int textureHeight;
        private float timer;

        private void Start()
        {
            textureWidth = visibleTilesX * pixelsPerTile;
            textureHeight = visibleTilesY * pixelsPerTile;

            texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;

            pixels = new Color[textureWidth * textureHeight];

            if (minimapImage != null)
                minimapImage.texture = texture;

            if (player == null)
            {
                var p = GameObject.FindGameObjectWithTag("Player");
                if (p != null) player = p.transform;
            }

            Redraw();
        }

        private void Update()
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                timer = updateInterval;
                Redraw();
            }
        }

        private void Redraw()
        {
            if (player == null || texture == null)
            {
                Debug.LogWarning($"Redraw fail: player={player}, texture={texture}");
                return;
            }

            var allMarkers = FindObjectsOfType<MinimapMarker>();
            Debug.Log($"Redraw: {allMarkers.Length} markers, playerPos={player.position}");
            if (player == null || texture == null) return;

            // 1. Заливаем фон
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = background;

            // 2. Границы мира-миникарты
            Vector2 playerPos = player.position;
            float halfW = visibleTilesX * 0.5f;
            float halfH = visibleTilesY * 0.5f;
            Vector2 worldMin = playerPos - new Vector2(halfW, halfH);
            Vector2 worldMax = playerPos + new Vector2(halfW, halfH);

            // 3. Рисуем маркеры
            
            foreach (var marker in allMarkers)
            {
                if (marker == null) continue;

                Vector2 pos = marker.transform.position;
                if (pos.x < worldMin.x || pos.x > worldMax.x) continue;
                if (pos.y < worldMin.y || pos.y > worldMax.y) continue;

                int px = Mathf.FloorToInt((pos.x - worldMin.x) * pixelsPerTile);
                int py = Mathf.FloorToInt((pos.y - worldMin.y) * pixelsPerTile);

                DrawSquare(px, py, 12, GetColor(marker.Type));
            }

            // 4. Игрок — в центре
            DrawSquare(textureWidth / 2, textureHeight / 2, 14, playerColor);

            texture.SetPixels(pixels);
            texture.Apply(false);
        }

        private void DrawSquare(int cx, int cy, int size, Color color)
        {
            int half = size / 2;
            for (int x = -half; x <= half; x++)
            {
                for (int y = -half; y <= half; y++)
                {
                    int px = cx + x;
                    int py = cy + y;
                    if (px < 0 || px >= textureWidth) continue;
                    if (py < 0 || py >= textureHeight) continue;
                    pixels[py * textureWidth + px] = color;
                }
            }
        }

        private Color GetColor(MinimapMarker.MarkerType type)
        {
            return type switch
            {
                MinimapMarker.MarkerType.Tree => treeColor,
                MinimapMarker.MarkerType.Rock => rockColor,
                MinimapMarker.MarkerType.Zombie => zombieColor,
                MinimapMarker.MarkerType.Player => playerColor,
                _ => Color.white
            };
        }
    }
}