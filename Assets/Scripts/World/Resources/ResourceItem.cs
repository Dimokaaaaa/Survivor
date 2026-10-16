using UnityEngine;
using Minecraft2D.InventorySystem;

namespace Minecraft2D.World.Resources
{
    /// <summary>
    /// Летающий дроп ресурса, который можно подобрать.
    /// </summary>
    public class ResourceItem : MonoBehaviour
    {
        [SerializeField] private ResourceType resourceType;
        [SerializeField] private int amount = 1;
        [SerializeField] private float pickupDistance = 0.7f;
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float lifetime = 60f;

        private Transform player;
        private bool isPickedUp;

        public ResourceType Type => resourceType;
        public int Amount => amount;

        public void Setup(ResourceType type, int count)
        {
            resourceType = type;
            amount = count;
        }

        private void Start()
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;

            Destroy(gameObject, lifetime);
        }

        private void Update()
        {
            if (isPickedUp || player == null) return;

            float dist = Vector2.Distance(transform.position, player.position);

            // Магнит — притягиваемся к игроку в радиусе 2 юнитов
            if (dist < 2f)
            {
                transform.position = Vector2.MoveTowards(
                    transform.position, player.position, moveSpeed * Time.deltaTime);
            }

            // Подбираем
            if (dist < pickupDistance)
                Pickup();
        }

        private void Pickup()
        {
            if (isPickedUp) return;
            isPickedUp = true;

            Inventory.Instance?.Add(resourceType, amount);
            Destroy(gameObject);
        }
    }
}