using Codice.Client.BaseCommands;
using silly;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.YamlDotNet.Core.Events;
using UnityEngine;

namespace Silly
{
    public enum AnimalState
    {
        Idle,
        Move,
        Stop
    }

    public class Animal : MonoBehaviour
    {
        public float moveSpeed = 2f;
        AnimalState state = AnimalState.Idle;
        [SerializeField]
        Vector2Int currentPos;
        Vector2Int currentDir = Vector2Int.right;
        bool moving;
        [SerializeField]
        private bool isMeeting;
        Vector2Int[] dirs =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };
        private Coroutine moveCoroutine;

        private void Start()
        {
            currentPos = new Vector2Int((int)this.transform.position.x, (int)this.transform.position.z);
            MapController.Instance.animalMap[currentPos.x, currentPos.y] = this;
            StartCoroutine(AI());
        }

        void ChangeDirection()
        {
            List<Vector2Int> candidates = new List<Vector2Int>();

            foreach (var dir in dirs)
            {
                // 현재 방향은 제외
                if (dir == currentDir)
                    continue;

                Vector2Int next = currentPos + dir;

                if (MapController.Instance.CanMove(next))
                {
                    candidates.Add(dir);
                }

            }

            if (candidates.Count > 0)
            {
                currentDir = candidates[Random.Range(0, candidates.Count)];
            }
        }

        IEnumerator AI()
        {
            while (true)
            {
                switch (state)
                {
                    case AnimalState.Idle:

                        yield return new WaitForSeconds(Random.Range(3f, 6f));

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
                }

                yield return null;
            }
        }


        void MoveAI()
        {
            // 80% 직진
            if (Random.value > 0.8f)
            {
                currentDir = dirs[Random.Range(0, dirs.Length)];
            }

            Vector2Int next = currentPos + currentDir;
            FaceDirection(currentDir);

            if (!MapController.Instance.CanMove(next))
            {
                ChangeDirection();
                return;
            }

            MapController.Instance.Reserve(this, next);
            moveCoroutine = StartCoroutine(Move(next));
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
            else
            {
                this.transform.GetChild(0).LookAt(new Vector3(dir.x, 0, dir.y));
            }
        }

        IEnumerator Move(Vector2Int next)
        {
            moving = true;

            Vector2Int oldPos = currentPos;

            Vector3 target = GridToWorld(next);

            while (Vector3.Distance(transform.position, target) > 0.01f && state != AnimalState.Stop)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    target,
                    moveSpeed * Time.deltaTime);

                yield return null;
            }

            currentPos = next;

            MapController.Instance.CompleteMove(this, oldPos, next);

            moving = false;

        }


        Vector3 GridToWorld(Vector2Int grid)
        {
            return new Vector3(grid.x, 0, grid.y);
        }

        Vector2Int WorldToGrid(Vector3 world)
        {
            return new Vector2Int(
                Mathf.RoundToInt(world.x),
                Mathf.RoundToInt(world.y));
        }

        public void SetCurrentPos(Vector3 worldPos)
        {
            Vector2Int preCurrentPos = currentPos;
            //Debug.Log(preCurrentPos);
            currentPos = new Vector2Int(Mathf.RoundToInt(worldPos.x), Mathf.RoundToInt(worldPos.z));
            if (!MapController.Instance.OutMap(currentPos) )
            {
                //Debug.Log("동작");
                currentPos = preCurrentPos;
            }
            state = AnimalState.Stop;
            MapController.Instance.CompleteMove(this, preCurrentPos, currentPos);
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
                moveCoroutine = null;
                moving = false;
            }
            this.transform.position = new Vector3(currentPos.x, 0, currentPos.y);

        }


        public void Meet(Animal other)
        {

            if (isMeeting || other.isMeeting)
            {
                return;
            }
            PlayerController.Instance.SetStopDrag();
            StartCoroutine(MeetRoutine(other));
        }

        IEnumerator MeetRoutine(Animal other)
        {
            isMeeting = true;
            state = AnimalState.Stop;
            other.state = AnimalState.Stop;

            // 여기서 이벤트 실행
            //EventManager.Instance.OnAnimalMeet(this, other);
            
            // 서로 바라보기
            //LookAt(other.currentPos);
            //other.LookAt(currentPos);
            FaceDirection(other.currentPos);
            other.FaceDirection(currentPos);

            yield return new WaitForSeconds(2f);

            ChangeDirection();
            other.ChangeDirection();

            state = AnimalState.Idle;
            other.state = AnimalState.Idle;

            isMeeting = false;
        }

        //public void LookAt(Vector2Int target)
        //{
        //    Vector2Int dir = target - currentPos;

        //    if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        //    {
        //        if(dir.x > 0)
        //        {
        //            currentDir = Vector2Int.right;
        //        }
        //        else
        //        {
        //            currentDir = Vector2Int.left;
        //        }
        //    }
        //    else
        //    {
        //        if (dir.y > 0)
        //        {
        //            currentDir = Vector2Int.up;
        //        }
        //        else
        //        {
        //            currentDir = Vector2Int.down;
        //        }
        //    }

        //}

        private void OnTriggerEnter(Collider other)
        {
            
            if (other.gameObject.CompareTag("Animal"))
            {
                Debug.Log(this.gameObject.name + " meet " + other.gameObject.name);
                Meet(other.GetComponent<Animal>());
            }
        }
    }
}

    