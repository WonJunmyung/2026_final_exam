using UnityEngine;

namespace Silly
{
    public class Util
    {
        public static Vector3 GridToWorld(Vector2Int grid)
        {
            return new Vector3(grid.x, 0, grid.y);
        }

        public static Vector2Int WorldToGrid(Vector3 world)
        {
            return new Vector2Int(
                Mathf.RoundToInt(world.x),
                Mathf.RoundToInt(world.z));
        }

    }
}
