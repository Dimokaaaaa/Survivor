using UnityEngine;
using UnityEngine.Tilemaps;

namespace Minecraft2D.World
{
    /// <summary>
    /// Генерирует сетку через Tilemap. Один клик — вся карта.
    /// </summary>
    [RequireComponent(typeof(Tilemap))]
    public class GridGenerator : MonoBehaviour
    {
        [Header("Размер сетки (в тайлах)")]
        [SerializeField] private int width = 100;
        [SerializeField] private int height = 100;

        [Header("Тайл сетки")]
        [SerializeField] private TileBase gridTile;

        [Header("Центрировать вокруг (0,0)")]
        [SerializeField] private bool centerAtOrigin = true;

        [ContextMenu("Generate Grid")]
        private void Generate()
        {
            var tilemap = GetComponent<Tilemap>();
            tilemap.ClearAllTiles();

            if (gridTile == null)
            {
                Debug.LogError("GridGenerator: gridTile не задан!");
                return;
            }

            int startX = centerAtOrigin ? -width / 2 : 0;
            int startY = centerAtOrigin ? -height / 2 : 0;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector3Int pos = new Vector3Int(startX + x, startY + y, 0);
                    tilemap.SetTile(pos, gridTile);
                }
            }

            Debug.Log($"GridGenerator: создано {width * height} тайлов");
        }

        [ContextMenu("Clear Grid")]
        private void Clear()
        {
            var tilemap = GetComponent<Tilemap>();
            tilemap.ClearAllTiles();
            Debug.Log("GridGenerator: сетка очищена");
        }
    }
}