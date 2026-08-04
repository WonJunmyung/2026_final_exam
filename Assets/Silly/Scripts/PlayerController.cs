using UnityEngine;
using UnityEngine.EventSystems;
namespace silly
{
    public class PlayerController : MonoBehaviour
    {
        [Header("카메라 설정")]
        public Camera cam;
        public float moveSpeed = 15f;
        public float zoomSpeed = 20f;
        public float minHeight = 8f;
        public float maxHeight = 30f;

        [Header("맵 제한")]
        public Vector2 limitX = new Vector2(-50f, 50f);
        public Vector2 limitZ = new Vector2(-50f, 50f);

        [Header("레이어")]
        public LayerMask draggableLayer;
        public LayerMask groundLayer;

        [Header("스크롤")]
        public bool edgeScroll = true;
        public float edgeSize = 15f;

        private Transform dragging;
        private Plane dragPlane;
        private Vector3 offset;

        void Awake()
        {
            if (cam == null)
            {
                cam = GameObject.Find("Main Camera").GetComponent<Camera>();
                //cam = Camera.main;
            }
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
            transform.position = transform.position + dir * moveSpeed * Time.deltaTime;

            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, limitX.x, limitX.y);
            pos.z = Mathf.Clamp(pos.z, limitZ.x, limitZ.y);

            Debug.Log(pos);
            transform.position = pos;
        }

        void CameraZoom()
        {
            float mouseScroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(mouseScroll) < 0.001f)
            {
                return;
            }
            Vector3 pos = transform.position;
            pos.y = pos.y - mouseScroll * zoomSpeed;
            pos.y = Mathf.Clamp(pos.y, minHeight, maxHeight);
            transform.position = pos;
        }

        void DragUpdate()
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, 500f, draggableLayer))
                {
                    dragging = hit.transform;
                    dragPlane = new Plane(Vector3.up, dragging.position);
                    if (dragPlane.Raycast(ray, out float d))
                    {
                        offset = dragging.position - ray.GetPoint(d);
                    }
                }
            }

            if (Input.GetMouseButton(0) && dragging != null)
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
                    dragging.position = pos;
                }
            }

            if (Input.GetMouseButtonUp(0))
                dragging = null;
        }
    }
}