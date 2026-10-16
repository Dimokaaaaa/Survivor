using UnityEngine;

namespace Minecraft2D.Player
{
    /// <summary>
    /// Управляет анимациями игрока.
    /// Читает ввод, решает какое состояние показать, передаёт в Animator.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(PlayerInputReader))]
    public class PlayerAnimator : MonoBehaviour
    {
        private static readonly int MoveXHash = Animator.StringToHash("MoveX");
        private static readonly int MoveYHash = Animator.StringToHash("MoveY");
        private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");

        private Animator animator;
        private PlayerInputReader input;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            input = GetComponent<PlayerInputReader>();
        }

        private void Update()
        {
            Vector2 move = input.MoveInput;

            animator.SetFloat(MoveXHash, move.x);
            animator.SetFloat(MoveYHash, move.y);
            animator.SetBool(IsMovingHash, move.sqrMagnitude > 0.01f);
        }
    }
}