using System.Collections.Generic;
using UnityEngine;
using Minecraft2D.UI;

namespace Minecraft2D.World
{
    public class WorkbenchInteraction : MonoBehaviour
    {
        [SerializeField] private float interactionRadius = 2f;

        public static readonly List<WorkbenchInteraction> AllWorkbenches = new();

        private Transform player;
        private bool playerNearby;

        private void OnEnable() => AllWorkbenches.Add(this);
        private void OnDisable() => AllWorkbenches.Remove(this);

        private void Start()
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        private void Update()
        {
            if (player == null) return;

            float dist = Vector2.Distance(transform.position, player.position);
            bool nearby = dist <= interactionRadius;

            if (nearby && !playerNearby)
            {
                playerNearby = true;
                CraftingUI.Instance?.ShowWorkbenchHint(true);
            }
            else if (!nearby && playerNearby)
            {
                playerNearby = false;
                CraftingUI.Instance?.ShowWorkbenchHint(false);
            }

            if (nearby && Input.GetKeyDown(KeyCode.E))
            {
                CraftingUI.Instance?.ToggleCraftingNearWorkbench();
            }
        }

        /// <summary>
        /// Проверяет, находится ли игрок рядом с ЛЮБЫМ верстаком.
        /// </summary>
        public static bool IsPlayerNearAnyWorkbench(Vector2 playerPos)
        {
            foreach (var wb in AllWorkbenches)
            {
                if (wb == null) continue;
                if (Vector2.Distance(wb.transform.position, playerPos) <= wb.interactionRadius)
                    return true;
            }
            return false;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionRadius);
        }
    }
}