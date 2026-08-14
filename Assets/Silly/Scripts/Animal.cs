using JetBrains.Annotations;
using silly;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Silly
{
    [System.Serializable]
    public enum AnimalState
    {
        Idle,           // 대기
        Move,           // 이동
        Eat,            // 먹으러 이동
        Eating,         // 먹기
        Drink,          // 마시러이동
        Drinking,       // 마시기        
        Rest,           // 휴식하러 이동
        Resting,        // 휴식
        Stop,           // 동작 중단
        Drag,           // 드래그 되는 상태
    }

    

    public class Animal : MonoBehaviour
    {
        public float moveSpeed = 2f;
        public int exp;
        public int lv = 1;
        public int hunger = 50;
        public int water = 50;
        public int hp = 100;
        public int waterCount = 0;


        public AnimalState state = AnimalState.Idle;
        [SerializeField]
        Vector2Int currentPos;
        Vector2Int currentDir = Vector2Int.right;
        bool moving;
        private bool isMeeting;
        Vector2Int[] dirs =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };
        private Coroutine moveCoroutine;
        Vector2Int next;

        public Vector2Int HungerPos;

        private float lastMeetTime = -999f;

        public float MeetCooldown = 10f;

        public LayerMask eventLayer;


        public AnimalName animalName = AnimalName.Dog;

        
        public float animalTime = 0f;
        public  float timeCal = 24f; // 현실 1분 = 게임 속 24분, 현실 1시간(3600초) = 게임 속 24시간(1440분).

        private void Start()
        {
            currentPos = new Vector2Int((int)this.transform.position.x, (int)this.transform.position.z);
            MapController.Instance.animalMap[currentPos.x, currentPos.y] = this;
            StartCoroutine(AI());
        }

        public void Update()
        {
            if (state != AnimalState.Drag)
            {
                TimeEvent();
            }
        }

        public void TimeEvent()
        {
            if(state == AnimalState.Drag)
            {
                return;
            }
            animalTime += Time.deltaTime * timeCal;
            int minute = Mathf.FloorToInt(((float)animalTime % 3600f) / 60f);
            
            if (minute >= 30)
            {
                if (state != AnimalState.Drinking)
                {
                    water = water - 10;
                }
                if (state != AnimalState.Resting)
                {
                    hp = hp - 5;
                }
                if(water < 0)
                {
                    water = 0;
                }
                if(hp < 0)
                {
                    hp = 0;
                }
                animalTime = 0;
                waterCount++;
                if(waterCount == 2)
                {
                    waterCount = 0;
                    if (state != AnimalState.Eating)
                    {
                        hunger = hunger - 10;
                    }
                    if(hunger < 0)
                    {
                        hunger = 0;
                    }
                }
            }
        }

        // 목적지와 가까운 방향으로 이동
        void ChangeDirection(Vector2Int pos)
        {
            List<Vector2Int> candidates = new List<Vector2Int>();

            if (pos.x - currentPos.x < 0)
            {
                candidates.Add(Vector2Int.left);
            }
            else if(pos.x - currentPos.x > 0)
            {
                candidates.Add(Vector2Int.right);
            }
            else
            {
                candidates.Add(Vector2Int.left);
                candidates.Add(Vector2Int.right);
            }

            if (pos.y - currentPos.y < 0)
            {
                candidates.Add(Vector2Int.down);
            }
            else if (pos.y - currentPos.y > 0)
            {
                candidates.Add(Vector2Int.up);
            }
            else
            {
                candidates.Add(Vector2Int.up);
                candidates.Add(Vector2Int.down);
            }

            if (candidates.Count > 0)
            {
                currentDir = candidates[UnityEngine.Random.Range(0, candidates.Count)];
            }
        }


        void ChangeDirection()
        {
            List<Vector2Int> candidates = new List<Vector2Int>();

            foreach (var dir in dirs)
            {
                // 현재 방향은 제외
                if (dir == currentDir)
                {
                    continue;
                }

                Vector2Int next = currentPos + dir;

                if (MapController.Instance.CanMove(next))
                {
                    candidates.Add(dir);
                }

            }

            if (candidates.Count > 0)
            {
                currentDir = candidates[UnityEngine.Random.Range(0, candidates.Count)];
            }
        }

        IEnumerator AI()
        {
                        
            while (true)
            {
                if(state == AnimalState.Drag)
                {

                }
                else if(state == AnimalState.Eating || state == AnimalState.Drinking || state == AnimalState.Resting)
                {

                }
                else if(hp == 0)
                {
                    Destroy(this.gameObject);
                }
                else if(hunger == 0 || water == 0)
                {
                    state = AnimalState.Stop;
                }
                
                else if (hunger < 30)
                {
                    state = AnimalState.Eat;
                }
                else if (water < 30)
                {
                    state = AnimalState.Drink;
                }
                else if (hp < 30)
                {
                    state = AnimalState.Rest;
                }
                

                switch (state)
                {
                    case AnimalState.Idle:

                        //yield return new WaitForSeconds(Random.Range(3f, 6f));
                        yield return new WaitForSeconds(1f);
                        state = AnimalState.Move;
                        break;

                    case AnimalState.Move:

                        MoveAI();

                        while (moving)
                        {
                            yield return null;
                        }

                        break;

                    case AnimalState.Stop:

                        yield return new WaitForSeconds(2);
                        state = AnimalState.Idle;

                        break;
                    case AnimalState.Drinking:
                        break;
                    case AnimalState.Drink:
                        HungerPos = MapController.Instance.GetHunger(currentPos, BuildData.WaterBox);
                        HungerAI(HungerPos);
                        while (moving)
                        {
                            yield return null;
                        }
                        break;
                    case AnimalState.Eating:
                        break;

                    case AnimalState.Eat:
                        HungerPos = MapController.Instance.GetHunger(currentPos, BuildData.EatBox);
                        HungerAI(HungerPos);
                        while (moving)
                        {
                            yield return null;
                        }
                        break;
                    case AnimalState.Rest:
                        HungerPos = MapController.Instance.GetHunger(currentPos, BuildData.Tree);
                        HungerAI(HungerPos);
                        while (moving)
                        {
                            yield return null;
                        }
                        break;
                    case AnimalState.Resting:
                        break;
                    case AnimalState.Drag:

                        break;
                }
                yield return null;
            }
        }
        

        void MoveAI()
        {
            if (moving)
            {
                return;
            }
            if (UnityEngine.Random.value > 0.8f)
            {
                ChangeDirection();
            }

            Vector2Int next = currentPos + currentDir;

            // 이동 방향 회전
            FaceDirection(currentDir);
            if (!MapController.Instance.RequestMove(this, currentPos, next))
            {
                ChangeDirection();
                return;
            }
            moveCoroutine = StartCoroutine(Move(next));
        }

        void HungerAI(Vector2Int targetPos)
        {
            if (moving)
            {
                return;
            }
            if (targetPos == new Vector2Int(-1, -1))
            {
                return;
            }
            ChangeDirection(targetPos);
            Vector2Int next = currentPos + currentDir;

            // 이동 방향 회전
            FaceDirection(currentDir);
            if (!MapController.Instance.RequestMove(this, currentPos, next))
            {
                StopMove();
                return;
            }
            moveCoroutine = StartCoroutine(Move(next));
        }


        GameObject FindObject()
        {
            if(state == AnimalState.Drag)
            {
                return null;
            }
            if (Time.time - lastMeetTime < MeetCooldown)
            {
                return null;
            }

            Vector3[] directions = { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };
            foreach (Vector3 dir in directions)
            {
                RaycastHit[] hits = Physics.RaycastAll(transform.GetChild(0).position, dir, 1, eventLayer);

                bool hitObstacle = false;
                RaycastHit realHit = new RaycastHit();

                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider != this.GetComponent<Collider>())
                    {
                        hitObstacle = true;
                        realHit = hit;
                        break; 
                    }
                }
                
                if (hitObstacle)
                {
                    if (realHit.transform.CompareTag("Animal"))
                    {
                        Animal animalHit = realHit.collider.transform.GetComponent<Animal>();
                        if (Time.time - animalHit.lastMeetTime < MeetCooldown)
                        {
                            return null;
                        }
                        if(animalHit.state == AnimalState.Drag)
                        {
                            return null;
                        }
                        lastMeetTime = Time.time;
                        animalHit.lastMeetTime = Time.time;
                    }
                    
                    Debug.DrawRay(transform.GetChild(0).position, dir, Color.red);
                    Debug.Log(" 감지: " + realHit.collider.name);
                    return realHit.transform.gameObject;
                }
                else
                {
                    Debug.DrawRay(transform.GetChild(0).position, dir, Color.green);
                }
            }

            return null;
        }



        void FaceDirection(Vector2Int dir)
        {
            if (dir == Vector2Int.right)
            {
                this.transform.GetChild(0).rotation = Quaternion.Euler(0, 90, 0);
            }
            else if (dir == Vector2Int.left)
            {
                this.transform.GetChild(0).rotation = Quaternion.Euler(0, -90, 0);
            }
            else if (dir == Vector2Int.up)
            {
                this.transform.GetChild(0).rotation = Quaternion.Euler(0, 0, 0);
            }
            else if (dir == Vector2Int.down)
            {
                this.transform.GetChild(0).rotation = Quaternion.Euler(0, 180, 0);
            }
        }

        IEnumerator Move(Vector2Int next)
        {
            moving = true;
            
            Vector2Int oldPos = currentPos;
            MapController.Instance.CompleteMove(this, oldPos, next);
            Vector3 target = GridToWorld(next);

            //Debug.Log(
            //    $"[MOVE START] {name} " +
            //    $"current={currentPos} -> next={next}"
            //);
            GameObject other = null;
            while (Vector3.Distance(transform.position, target) > 0.01f && state != AnimalState.Stop)
            {
                transform.position = Vector3.MoveTowards(this.transform.position, target, moveSpeed * Time.deltaTime);
                
                other = FindObject();
                if(other != null)
                {
                    StopMove();
                    if (other.transform.CompareTag("Animal"))
                    {
                        Animal otherAnimal = other.GetComponent<Animal>();
                        otherAnimal.StopMove();
                        other.transform.GetChild(0).LookAt(this.transform.GetChild(0));
                        
                        if(exp >= 1000 && otherAnimal.exp >= 1000)
                        {
                            UIMenu.Instance.OpenLv(lv);
                        }
                        else
                        {
                            exp += 2;
                            if (exp >= 1000)
                            {
                                exp = 1000;
                            }
                            other.GetComponent<Animal>().exp += 2;
                            if (other.GetComponent<Animal>().exp >= 1000)
                            {
                                other.GetComponent<Animal>().exp = 1000;
                            }
                        }
                    }
                    else if(other.transform.CompareTag("Building"))
                    {
                        other.GetComponent<Building>().tag = "NoneBuilding";
                        BuildData tempBuild = other.GetComponent<Building>().buildData;
                        switch (tempBuild)
                        {
                            case BuildData.EatBox:
                                exp += 5;
                                if (exp >= 1000)
                                {
                                    exp = 1000;
                                }
                                break;
                            case BuildData.WaterBox:
                                exp += 3;
                                if (exp >= 1000)
                                {
                                    exp = 1000;
                                }
                                break;
                            case BuildData.Tree:
                                exp += 4;
                                if (exp >= 1000)
                                {
                                    exp = 1000;
                                }
                                break;
                        }
                    }

                    this.transform.GetChild(0).LookAt(other.transform.GetChild(0));
                    
                    break;
                }
                
                yield return null;
            }
            if (other == null)
            {
                this.transform.position = target;

                currentPos = next;

                exp += 1;
                if(exp >= 1000)
                {
                    exp = 1000;
                }
                //MapController.Instance.CompleteMove(this, oldPos, next);
                
                moving = false;
                moveCoroutine = null;
            }
            
        }




        Vector3 GridToWorld(Vector2Int grid)
        {
            return new Vector3(grid.x, 0, grid.y);
        }

        Vector2Int WorldToGrid(Vector3 world)
        {
            return new Vector2Int(
                Mathf.RoundToInt(world.x),
                Mathf.RoundToInt(world.z));
        }

        public void SetCurrentPos(Vector3 worldPos)
        {
            Vector2Int preCurrentPos = currentPos;

            //currentPos = new Vector2Int(Mathf.RoundToInt(worldPos.x), Mathf.RoundToInt(worldPos.z));
            currentPos = WorldToGrid(worldPos);

            if (!MapController.Instance.OutMap(currentPos) )
            {
                currentPos = preCurrentPos;
            }
            state = AnimalState.Stop;
            MapController.Instance.CompleteMove(this, preCurrentPos, currentPos, next);
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
                moveCoroutine = null;
                moving = false;
            }

            //this.transform.position = new Vector3(currentPos.x, 0, currentPos.y);
            this.transform.position = GridToWorld(currentPos);


        }



        public void StopMove()
        {
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
                moveCoroutine = null;
            }
            state = AnimalState.Stop;
            moving = false;
        }

        public void MeetBuilding(Building other)
        {
            PlayerController.Instance.SetStopDrag();
            StartCoroutine(PlayBuilding(other));
        }



        IEnumerator PlayBuilding(Building other)
        {
            isMeeting = true;
            state = AnimalState.Stop;


            // 여기서 이벤트 실행
            other.PlayBuilding(this);

            yield return new WaitForSeconds(2f);

            ChangeDirection();

            state = AnimalState.Idle;

            isMeeting = false;
        }

        
    }
}

    