using UnityEngine;
using UnityEngine.EventSystems;

namespace Muppets
{
    public class Puppeteer : MonoBehaviour
    {
        public GameObject Puppet;

        public Camera Camera;

        Vector2 Since;

        public void OnDown(Vector2 position)
        {
            if (Input.touchCount == 1 || Input.GetMouseButton(0))
            {
                Since = position;
            }
        }

        public void OnDrag(Vector2 position)
        {
            if (Since != Vector2.zero && Since != position)
            {
                #if !UNITY_EDITOR
                if (Input.touchCount > 1)
                {
                    return;
                }
                #endif

                // 押下時からの差分を取得.
                Vector2 diff = position - Since;

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
            }
        }

        public void OnUp()
        {
            Since = Vector2.zero;
            ExecuteEvents.Execute<Puppet>(Puppet, null, (a, b) => a.OnMove(Vector3.zero));
        }
    }
}
