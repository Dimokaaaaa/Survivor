using UnityEngine;

namespace Minecraft2D.Player
{
    /// <summary>
    /// Зеркалит спрайт игрока при движении влево.
    /// Если игрок движется вверх — не зеркалим (вид сзади симметричен).
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(PlayerInputReader))]
    public class PlayerSpriteFlipper : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        private PlayerInputReader input;
        private bool facingLeft;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            input = GetComponent<PlayerInputReader>();
        }

        private void Update()
        {
            Vector2 move = input.MoveInput;

            // Если движется строго вверх или вниз — направление по X не меняем
            if (Mathf.Abs(move.x) < 0.01f) return;

            bool shouldFaceLeft = move.x < 0;
            if (shouldFaceLeft == facingLeft) return;

            facingLeft = shouldFaceLeft;
            spriteRenderer.flipX = facingLeft;
        }
    }
}