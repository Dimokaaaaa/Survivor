using UnityEngine;

namespace Minecraft2D.Player
{
    /// <summary>
    /// Читает ввод игрока с клавиатуры (WASD и стрелки).
    /// Возвращает вектор направления в 2D.
    /// </summary>
    public class PlayerInputReader : MonoBehaviour
    {
        /// <summary>
        /// Направление движения. Диапазон X: -1..1, Y: -1..1.
        /// Диагональ даёт (1,1) — нормализацию делает PlayerMovement.
        /// </summary>
        public Vector2 MoveInput { get; private set; }

        private void Update()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            MoveInput = new Vector2(horizontal, vertical);
        }
    }
}