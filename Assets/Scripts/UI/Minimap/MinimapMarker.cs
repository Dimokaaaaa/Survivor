using UnityEngine;

namespace Minecraft2D.UI
{
    public class MinimapMarker : MonoBehaviour
    {
        public enum MarkerType { Tree, Rock, Zombie, Player, Other }
        [SerializeField] private MarkerType type = MarkerType.Other;
        public MarkerType Type => type;
    }
}