using Silly;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;
namespace silly
{
    public enum PlayerState
    {
        Dragble,
        Store,
    }
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance;
        public Camera cam;
        public float moveSpeed = 15f;
        public float zoomSpeed = 20f;
        public float minHeight = 8f;
        public float maxHeight = 30f;

        public Vector2 limitX = new Vector2(-50f, 50f);
        public Vector2 limitZ = new Vector2(-50f, 50f);

        public LayerMask draggableLayer;
        public LayerMask groundLayer;

        public bool edgeScroll = true;
        public float edgeSize = 15f;
        /// <summary>
        /// 드래그될 오브젝트
        /// </summary>
        public Transform objDrag;
        private Plane dragPlane;
        private Vector3 offset;

        public PlayerState playerState = PlayerState.Dragble;


        /// <summary>
        /// 마우스를 따라다닐 건물 프리팹
        /// </summary>
        public GameObject buildingPrefab;
        /// <summary>
        /// 마우스를 따라 다닐 건물 오브젝트
        /// </summary>
        private GameObject previewObj;
        ///// <summary>
        ///// 생성될 건물의 데이터
        ///// </summary>
        //private Building buildingData;
        ///// <summary>
        ///// 마우스가 움직일때 생성 조건에 따라 변화되는 색깔 관리 스크립트
        ///// </summary>
        //private BuildingPreview previewScript;

        void Awake()
        {
            if (cam == null)
            {
                cam = GameObject.Find("Main Camera").GetComponent<Camera>();
                //cam = Camera.main;
            }
            Instance = this;
            
        }

        void Update()
        {
            CameraMove();
            CameraZoom();
            DragUpdate();
        }

        void CameraMove()
        {
            Vector3 dir = Vector3.zero;
            dir = dir + new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

            if (edgeScroll)
            {
                Vector3 mp = Input.mousePosition;
                if (mp.x <= edgeSize)
                {
                    dir.x = dir.x - 1;
                }
                if (mp.x >= Screen.width - edgeSize)
                {
                    dir.x = dir.x + 1;
                }
                if (mp.y <= edgeSize)
                {
                    dir.z = dir.z - 1;
                }
                if (mp.y >= Screen.height - edgeSize)
                {
                    dir.z = dir.z + 1;
                }
            }

            dir = dir.normalized;
            cam.transform.position = cam.transform.position + dir * moveSpeed * Time.deltaTime;

            Vector3 pos = cam.transform.position;
            pos.x = Mathf.Clamp(pos.x, limitX.x, limitX.y);
            pos.z = Mathf.Clamp(pos.z, limitZ.x, limitZ.y);

            cam.transform.position = pos;
        }

        void CameraZoom()
        {
            float mouseScroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(mouseScroll) < 0.001f)
            {
                return;
            }
            Vector3 pos = cam.transform.position;
            pos.y = pos.y - mouseScroll * zoomSpeed;
            pos.y = Mathf.Clamp(pos.y, minHeight, maxHeight);
            cam.transform.position = pos;
        }

        void DragUpdate()
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                playerState = PlayerState.Dragble;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                playerState = PlayerState.Store;
            }

                switch (playerState)
                {
                    case PlayerState.Dragble:
                        {
                            if (Input.GetMouseButtonDown(0))
                            {
                                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                                if (Physics.Raycast(ray, out RaycastHit hit, 500f, draggableLayer))
                                {
                                    objDrag = hit.transform;
                                    objDrag.GetComponent<BoxCollider>().enabled = false;
                                    dragPlane = new Plane(Vector3.up, objDrag.position);
                                    if (dragPlane.Raycast(ray, out float d))
                                    {
                                        offset = objDrag.position - ray.GetPoint(d);
                                    }
                                }
                            }

                            if (Input.GetMouseButton(0) && objDrag != null)
                            {
                                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                                if (dragPlane.Raycast(ray, out float d))
                                {
                                    Vector3 pos = ray.GetPoint(d) + offset;

                                    Ray down = new Ray(pos + Vector3.up * 100, Vector3.down);
                                    if (Physics.Raycast(down, out RaycastHit g, 200f, groundLayer))
                                    {
                                        pos.y = g.point.y;
                                    }
                                    objDrag.position = pos;
                                }
                            }

                            if (Input.GetMouseButtonUp(0) && objDrag != null)
                            {
                                objDrag.GetComponent<BoxCollider>().enabled = true;
                                if (objDrag.gameObject.CompareTag("Animal"))
                                {
                                    SetStopDrag();
                                }
                            }
                        }
                        break;
                    case PlayerState.Store:
                        {
                            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                            if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Floor")))
                            {

                                Vector3 hitPoint = hit.point;
                                Vector2Int gridPos = new Vector2Int(Mathf.FloorToInt(hitPoint.x), Mathf.FloorToInt(hitPoint.z));
                                Vector3 displayPos = new Vector3(gridPos.x + buildingData.size.x / 2f, 1, gridPos.y + buildingData.size.y / 2f);
                                previewObj.transform.position = displayPos;

                                //bool canPlace = GameManager.Instance.IsAreaFree(gridPos, buildingData.size);
                                previewScript.SetColor(canPlace ? Color.green : Color.red);

                                if (Input.GetMouseButtonDown(0) && canPlace)
                                {
                                    PlaceBuilding(gridPos);

                                }
                            }
                    }
                        break;
                }
            
        }

        public void SetStopDrag()
        {
            if (objDrag != null)
            {
                objDrag.GetComponent<Animal>().SetCurrentPos(objDrag.position);
                objDrag = null;
            }
        }

        public void StartBuilding(int buildingSize)
        {
            previewObj = Instantiate(buildingPrefab);
            //buildingData = previewObj.GetComponent<Building>();
            //buildingData.SetBuildingSize(buildingSize);
            //previewScript = previewObj.AddComponent<BuildingPreview>();
        }
    }
}