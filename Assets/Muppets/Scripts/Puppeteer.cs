using UnityEngine;
using UnityEngine.EventSystems;

namespace Muppets
{
    public class Puppeteer : MonoBehaviour
    {
        public GameObject Puppet;

        public Camera Camera;

        public float BeginDragDinstance = 0.1f;

        public float LongDownSeconds = 0.5f;

        Vector2 DownStartPosition;

        float DownStartTime;

        bool Down
        {
            get
            {
                return DownStartPosition != Vector2.zero &&
                DownStartTime != 0;
            }
            set
            {
                if (value)
                {
                    throw new UnityException("Unsupported operation.");
                }
                else
                {
                    DownStartPosition = Vector3.zero;
                    DownStartTime = 0;
                }
            }
        }

        bool LongDown;

        bool Move;

        void Update()
        {
            if (Down && !Move && Time.time - DownStartTime > LongDownSeconds)
            {
                ExecuteEvents.Execute<Puppet>(Puppet, null, (a, b) => a.OnLongDown());
                LongDown = true;
            }
        }

        public void OnDown(Vector2 position)
        {
            if (Input.touchCount == 1 || Input.GetMouseButton(0))
            {
                DownStartPosition = position;
                DownStartTime = Time.time;
            }
        }

        public void OnDrag(Vector2 position)
        {
            // 押下時からの差分を取得.
            Vector2 diff = position - DownStartPosition;

            float distance = diff.magnitude / Screen.dpi;

            if (distance > BeginDragDinstance)
            {
                #if !UNITY_EDITOR
                if (Input.touchCount > 1)
                {
                    return;
                }
                #endif

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
                }

                // 送信.
                ExecuteEvents.Execute<Puppet>(Puppet, null, (a, b) => a.OnMove(direction));

                Move = true;
            }
        }

        public void OnUp()
        {
            if (Move)
            {
                ExecuteEvents.Execute<Puppet>(Puppet, null, (a, b) => a.OnMove(Vector3.zero));
            }
            else if (!LongDown)
            {
                ExecuteEvents.Execute<Puppet>(Puppet, null, (a, b) => a.OnClick());
            }

            Down = LongDown = Move = false;
        }
    }
}
