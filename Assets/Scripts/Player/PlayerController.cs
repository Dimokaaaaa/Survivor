using UnityEngine;

namespace Minecraft2D.Player
{
    /// <summary>
    /// Главный компонент игрока. Координирует остальные, сам логики не содержит.
    /// </summary>
    [RequireComponent(typeof(PlayerInputReader))]
    [RequireComponent(typeof(PlayerMovement))]
    [RequireComponent(typeof(PlayerSpriteFlipper))]
    public class PlayerController : MonoBehaviour
    {
        // Пока пусто. Сюда будем добавлять здоровье, инвентарь и т.д.
    }
}