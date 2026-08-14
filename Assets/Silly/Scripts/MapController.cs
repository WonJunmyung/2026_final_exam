using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Silly
{
    public class MapController : MonoBehaviour
    {
        public static MapController Instance;
        public int[,] map;
        public int mapSize = 4;
        public Transform Map;
        public GameObject floorTile;
        public GameObject wallTile;
        public Animal[,] animalMap;
        public Animal[,] reserveMap;
        public GameObject[,] build;
        Vector2Int[] dirs =
        {
                Vector2Int.up,
                Vector2Int.down,
                Vector2Int.left,
                Vector2Int.right
        };
        private void Awake()
        {
            Instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            map = new int[mapSize, mapSize];
            build = new GameObject[mapSize, mapSize];
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
                        build[x, y] = Instantiate(wallTile, floorTilePos + new Vector3(0, 1, 0), floorTile.transform.rotation, Map);
                        
                    }
                    else
                    {
                        build[x, y] = Instantiate(floorTile, floorTilePos, floorTile.transform.rotation, Map);
                    }
                    
                }
            }
        }
       
        public bool OutMap(Vector2Int pos)
        {
            if (pos.x < 0 || pos.y < 0 ||
                pos.x >= map.GetLength(0) ||
                pos.y >= map.GetLength(1))
            {
                return false;
            }
            return true;
        }

        public bool CanMove(Vector2Int pos)
        {
            // 맵 밖
            if (!IsInsideMap(pos))
            {
                return false;
            }

            // 벽
            if (map[pos.x, pos.y] == 1)
            {
                return false;
            }

            // 다른 동물이 있음
            if (animalMap[pos.x, pos.y] != null)
            {
                return false;
            }
            // 예약 되어 있음
            if (reserveMap[pos.x, pos.y] != null)
            {
                return false;
            }

            return true;
        }

        public bool RequestMove(Animal animal, Vector2Int from, Vector2Int to)
        {
            if (animal == null)
            {
                return false;
            }

            if (!IsInsideMap(to))
            {
                return false;
            }

            // 벽
            if (map[to.x, to.y] == 1)
            {
                return false;
            }

            // 현재 다른 동물
            Animal target = animalMap[to.x, to.y];

            if (target != null && target != animal)
            {
                // 만남 판정도 여기서
                //HandleMeet(animal, target);

                return false;
            }

            // 다른 동물이 이미 예약
            if (reserveMap[to.x, to.y] != null)
                return false;

            // 예약
            reserveMap[to.x, to.y] = animal;

            return true;
        }


        public bool CanBuild(Vector2Int pos, Vector2Int size)
        {
            // 맵 밖
            if (pos.x < 0 || pos.y < 0 ||
                pos.x > map.GetLength(0) - (size.x) ||
                pos.y > map.GetLength(1) - (size.y))
            {
                return false;
            }

            //동물 관련
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    int tempX = pos.x + i;
                    int tempY = pos.y + j;
                    if (tempX < map.GetLength(0) && tempY < map.GetLength(1))
                    {
                        if (map[tempX, tempY] == 1)
                        {
                            return false;
                        }



                        //// 다른 동물이 있거나 예약되어 있음
                        //if (animalMap[tempX, tempY] != null)
                        //{
                        //    return false;
                        //}

                        //if (reserveMap[tempX, tempY] != null)
                        //{
                        //    return false;
                        //}
                    }
                }
            }

            return true;
        }
        public Animal GetAnimal(Vector2Int pos)
        {
            if (pos.x < 0 || pos.y < 0 || pos.x >= animalMap.GetLength(0) || pos.y >= animalMap.GetLength(1))
            {
                return null;
            }

            return animalMap[pos.x, pos.y];
        }



        public void CompleteMove(Animal animal, Vector2Int from, Vector2Int to)
        {
            reserveMap[to.x, to.y] = null;

            animalMap[from.x, from.y] = null;
            //build[from.x, from.y].transform.GetChild(0).GetComponent<Renderer>().material.color = UnityEngine.Color.aliceBlue;


            animalMap[to.x, to.y] = animal;
            //build[to.x, to.y].transform.GetChild(0).GetComponent<Renderer>().material.color = UnityEngine.Color.beige;

        }

        public void CompleteMove(Animal animal, Vector2Int from, Vector2Int to, Vector2Int next)
        {
            reserveMap[to.x, to.y] = null;

            animalMap[from.x, from.y] = null;
            //build[from.x, from.y].transform.GetChild(0).GetComponent<Renderer>().material.color = UnityEngine.Color.black;

            reserveMap[next.x, next.y] = null;

            animalMap[next.x, next.y] = null;
            //build[next.x, next.y].transform.GetChild(0).GetComponent<Renderer>().material.color = UnityEngine.Color.black;

            animalMap[to.x, to.y] = animal;
            //build[to.x, to.y].transform.GetChild(0).GetComponent<Renderer>().material.color = UnityEngine.Color.beige;
        }

        

        public List<Vector2Int> FindPath(Vector2Int start, Vector2Int target)
        {
            Queue<Vector2Int> queue = new Queue<Vector2Int>();

            Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();

            queue.Enqueue(start);
            cameFrom[start] = start;

            

            while (queue.Count > 0)
            {
                Vector2Int current = queue.Dequeue();

                if (current == target)
                {
                    break;
                }

                foreach (Vector2Int dir in dirs)
                {
                    Vector2Int next = current + dir;

                    if (!IsInsideMap(next))
                    {
                        continue;
                    }

                    // 벽
                    if (map[next.x, next.y] == 1)
                    {
                        continue;
                    }

                    // 이미 방문
                    if (cameFrom.ContainsKey(next))
                    {
                        continue;
                    }

                    queue.Enqueue(next);
                    cameFrom[next] = current;
                }
            }

            // 목적지까지 길이 없음
            if (!cameFrom.ContainsKey(target))
            {
                return null;
            }

            // 경로 복원
            List<Vector2Int> path = new List<Vector2Int>();

            Vector2Int pos = target;

            while (pos != start)
            {
                path.Add(pos);
                pos = cameFrom[pos];
            }

            path.Reverse();

            return path;
        }

        public bool IsInsideMap(Vector2Int pos)
        {
            if (pos.x >= 0 && pos.y >= 0 && pos.x < map.GetLength(0) && pos.y < map.GetLength(1))
            {
                return true;
            }
            return false;
        }

        public void CancelMove(Animal animal, Vector2Int pos)
        {
            if (!IsInsideMap(pos))
            {
                return;
            }
                

            if (reserveMap[pos.x, pos.y] == animal)
            {
                reserveMap[pos.x, pos.y] = null;
            }
        }

        
        public Vector2Int GetHunger(Vector2Int pos, BuildData buildData)
        {
            List<GameObject> objHunger = GameObject.FindGameObjectsWithTag("Building").ToList();

            List<GameObject> buildDataList = objHunger.FindAll(A=>A.GetComponent<Building>().buildData == buildData);

            if (buildDataList.Count < 1)
            {
                return -Vector2Int.one;
            }

            Vector2Int[] objHungerPos = new Vector2Int[buildDataList.Count];
            for(int i=0; i< buildDataList.Count; i++)
            {
                objHungerPos[i] = Util.WorldToGrid(buildDataList[i].transform.position);
            }

            float minDistance = Vector2.SqrMagnitude(objHungerPos[0] - pos);

            int num = 0;
            for(int i=1; i< buildDataList.Count; i++)
            {
                float distance = Vector2.SqrMagnitude(objHungerPos[i] - pos);
                if(distance < minDistance)
                {
                    minDistance = distance;
                    num = i;
                }
            }
            return objHungerPos[num];
        }

        public GameObject GetRandomPosition()
        {
            List<GameObject> list = new List<GameObject>();

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] != 1)
                    {
                        list.Add(build[i, j]);
                    }
                }
            }

            return list[Random.Range(0, list.Count)];
        }


    }


}
