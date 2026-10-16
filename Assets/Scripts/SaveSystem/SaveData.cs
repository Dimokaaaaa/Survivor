using System;
using System.Collections.Generic;

namespace Minecraft2D.Saving
{
    [Serializable]
    public class SaveData
    {
        public int worldSeed;
        public float playerX;
        public float playerY;
        public List<ResourceEntry> resources = new();
        public List<PlacedEntry> placedObjects = new();
    }

    [Serializable]
    public class ResourceEntry
    {
        public int type;
        public int amount;
    }

    [Serializable]
    public class PlacedEntry
    {
        public int type;        // 10 = Wall, 11 = House
        public float x;
        public float y;
        public float rotation;
    }
}