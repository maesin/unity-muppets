using UnityEngine;
using UnityEngine.EventSystems;

namespace Puppets
{
    public class Puppeteer : MonoBehaviour
    {
        public GameObject Puppet;

        public Camera Camera;

        public float BeginDragDistance = 0.1f;

        public float LongDownSeconds = 0.5f;

        public bool ClickRaycast;

        public LayerMask ClickRaycastLayerMask;

        Vector2 LastDownPosition;

        float LastDownTime;

        Vector3 LastDragDirection;

        bool Down
        {
            get
            {
                return LastDownPosition != Vector2.zero &&
                    LastDownTime != 0;
            }
            set
            {
                if (value)
                {
                    throw new UnityException("Unsupported operation.");
                }
                else
                {
                    LastDownPosition = Vector2.zero;
                    LastDownTime = 0;
                }
            }
        }

        int DownCount;

        bool LongDown;

        bool Move;

        void Update()
        {
            if (Down && DownCount == 1 && !Move && Time.time - LastDownTime > LongDownSeconds)
            {
                ExecuteEvents.Execute<Puppet>(Puppet, null, (a, b) => a.OnLongDown());
                LongDown = true;
            }
        }

        public void OnDown(Vector2 position)
        {
            if (DownCount == 0)
            {
                LastDownPosition = position;
                LastDownTime = Time.time;
            }

            DownCount++;
        }

        public void OnDrag(Vector2 position)
        {
            // 押下時からの差分を取得.
            Vector2 diff = position - LastDownPosition;

            bool detected;

            if (Move)
            {
                detected = diff != Vector2.zero;
            }
            else
            {
                detected = diff.magnitude / Screen.dpi > BeginDragDistance;
            }

            if (detected)
            {
                if (DownCount == 1)
                {
                    // アスペクト比の小さい方を操作範囲の一片とする.
                    int side = Mathf.Min(Screen.width, Screen.height) / 2;

                    // 正規化.
                    float x = Mathf.Clamp(diff.x / side, -1, 1);
                    float z = Mathf.Clamp(diff.y / side, -1, 1);

                    // 画面上の方向.
                    var direction = new Vector3(x, 0, z);

                    if (Camera != null)
                    {
                        // カメラの方向を考慮.
                        var cameraDirection = Quaternion.Euler(0, Camera.transform.eulerAngles.y, 0) * direction;

                        // 正規化.
                        // x か z を 1 と考えて正規化するため、Normalize でもいいが、負荷を考慮して簡易的に計算.
                        cameraDirection /= Mathf.Max(Mathf.Abs(cameraDirection.x), Mathf.Abs(cameraDirection.z));

                        // 移動操作の強度を反映.
                        cameraDirection *= Mathf.Max(Mathf.Abs(direction.x), Mathf.Abs(direction.z));

                        // 方向を更新.
                        direction = cameraDirection;

                        LastDragDirection = direction;
                    }
                }

                // 送信.
                ExecuteEvents.Execute<Puppet>(Puppet, null, (a, b) => a.OnMove(LastDragDirection));

                Move = true;
            }
        }

        public void OnUp(Vector2 position)
        {
            if (DownCount == 1)
            {
                LastDragDirection = Vector3.zero;

                if (Move)
                {
                    ExecuteEvents.Execute<Puppet>(Puppet, null, (a, b) => a.OnMove(LastDragDirection));
                }
                else if (!LongDown)
                {
                    if (ClickRaycast)
                    {
                        RaycastHit hit;
                        Ray ray = Camera.ScreenPointToRay(position);

                        if (Physics.Raycast(ray, out hit, Mathf.Infinity, ClickRaycastLayerMask))
                        {
                            ExecuteEvents.Execute<Puppet>(Puppet, null, (a, b) => a.OnClick(hit.point));
                        }
                    }
                    else
                    {
                        var near = new Vector3(position.x, position.y, Camera.nearClipPlane);
                        Vector3 world = Camera.ScreenToWorldPoint(near);
                        ExecuteEvents.Execute<Puppet>(Puppet, null, (a, b) => a.OnClick(world));
                    }
                }

                LongDown = Down = Move = false;
            }

            DownCount--;
        }
    }
}
