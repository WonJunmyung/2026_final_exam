using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Silly
{
    public class MapController : MonoBehaviour
    {
        public static MapController Instance;
        public int[,] map;
        int mapSize = 10;
        public Transform Map;
        public GameObject floorTile;
        public GameObject wallTile;
        public Animal[,] animalMap;
        public Animal[,] reserveMap;

        private void Awake()
        {
            Instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            map = new int[mapSize, mapSize];
            animalMap = new Animal[mapSize, mapSize];
            reserveMap = new Animal[mapSize, mapSize];
            SetTileMap();
        }

        // Update is called once per frame
        void Update()
        {
        
        }
        
        public void SetTileMap()
        {

            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    // map 값 할당
                    map[x, y] = 0;

                    Vector3Int floorTilePos = new Vector3Int(x, 0, y);
                    if (map[x, y] == 1)
                    {
                        Instantiate(floorTile, floorTilePos, floorTile.transform.rotation, Map);
                        Instantiate(wallTile, floorTilePos + new Vector3(0, 1, 0), floorTile.transform.rotation, Map);
                    }
                    else
                    {
                        Instantiate(floorTile, floorTilePos, floorTile.transform.rotation, Map);
                    }
                }
            }
        }
       
        public bool OutMap(Vector2Int pos)
        {
            if (pos.x < 0 || pos.y < 0 ||
                pos.x >= map.GetLength(0) ||
                pos.y >= map.GetLength(1))
                return false;
            return true;
        }

        public bool CanMove(Vector2Int pos)
        {
            // 맵 밖
            if (pos.x < 0 || pos.y < 0 ||
                pos.x >= map.GetLength(0) ||
                pos.y >= map.GetLength(1))
                return false;

            // 벽
            if (map[pos.x, pos.y] == 1)
                return false;

            // 다른 동물이 있거나 예약되어 있음
            if (animalMap[pos.x, pos.y] != null)
                return false;

            if (reserveMap[pos.x, pos.y] != null)
                return false;

            return true;
        }
        public bool Reserve(Animal animal, Vector2Int to)
        {
            // 맵 밖
            if (to.x < 0 || to.y < 0 ||
                to.x >= map.GetLength(0) ||
                to.y >= map.GetLength(1))
                return false;

            // 벽
            if (map[to.x, to.y] == 1)
                return false;

            // 다른 동물이 있으면 만남 이벤트
            //Animal other = animalMap[to.x, to.y];

            //if (other != null)
            //{
            //    animal.Meet(other);

            //    return false;
            //}

            // 예약된 타일
            if (reserveMap[to.x, to.y] != null)
                return false;

            reserveMap[to.x, to.y] = animal;

            return true;
        }
        public void CompleteMove(Animal animal, Vector2Int from, Vector2Int to)
        {
            reserveMap[to.x, to.y] = null;

            animalMap[from.x, from.y] = null;
            animalMap[to.x, to.y] = animal;
        }

    }
}
