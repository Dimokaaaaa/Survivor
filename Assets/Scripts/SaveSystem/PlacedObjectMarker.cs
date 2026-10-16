using UnityEngine;
using Minecraft2D.World.Resources;

namespace Minecraft2D.World
{
    /// <summary>
    /// Маркер, что объект был поставлен игроком (для сохранения).
    /// </summary>
    public class PlacedObjectMarker : MonoBehaviour
    {
        public ResourceType Type { get; private set; }

        public void Init(ResourceType type)
        {
            Type = type;
        }
    }
}