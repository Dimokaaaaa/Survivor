using UnityEngine;

namespace Minecraft2D.Utilities
{
    /// <summary>
    /// Генерирует текстуру-тайл для сетки мира.
    /// Запускается вручную через контекстное меню в инспекторе.
    /// </summary>
    public class GridTextureGenerator : MonoBehaviour
    {
        [Header("Настройки тайла")]
        [SerializeField] private int tileSize = 32;
        [SerializeField] private int borderThickness = 1;
        [SerializeField] private Color borderColor = new Color(0f, 0f, 0f, 0.35f);

        [ContextMenu("Generate Grid Tile PNG")]
        private void Generate()
        {
            Texture2D texture = new Texture2D(tileSize, tileSize, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Point;

            for (int x = 0; x < tileSize; x++)
            {
                for (int y = 0; y < tileSize; y++)
                {
                    bool isBorder =
                        x < borderThickness ||
                        y < borderThickness ||
                        x >= tileSize - borderThickness ||
                        y >= tileSize - borderThickness;

                    texture.SetPixel(x, y, isBorder ? borderColor : Color.clear);
                }
            }

            texture.Apply();

            byte[] png = texture.EncodeToPNG();
            string folder = Application.dataPath + "/Sprites/UI/";
            string path = folder + "grid_tile.png";

            System.IO.Directory.CreateDirectory(folder);
            System.IO.File.WriteAllBytes(path, png);
            Debug.Log($"Сетка сохранена: {path}");
        }
    }
}