using UnityEngine;

namespace Minecraft2D.Player
{
    /// <summary>
    /// Двигает игрока в 8 направлениях.
    /// Диагональ не быстрее, чем прямая — нормализация вектора.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Настройки")]
        [SerializeField] private float moveSpeed = 5f;

        private Rigidbody2D rb;
        private PlayerInputReader input;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            input = GetComponent<PlayerInputReader>();
        }

        private void FixedUpdate()
        {
            Vector2 direction = input.MoveInput;

            // Если игрок жмёт две кнопки — диагональ.
            // Без нормализации она была бы в 1.41 раза быстрее.
            if (direction.sqrMagnitude > 1f)
                direction = direction.normalized;

            rb.velocity = direction * moveSpeed;
            Vector2 pos = rb.position;
            pos.x = Mathf.Clamp(pos.x, -49f, 49f);
            pos.y = Mathf.Clamp(pos.y, -49f, 49f);
            rb.position = pos;
        }
    }
}