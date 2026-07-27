using Unity.Netcode;
using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour
{
    public Transform player;                 // 玩家角色的 Transform
    public float mouseSensitivity = 100f;    // 鼠标灵敏度
    public float distanceFromPlayer = 4f;    // 摄像机与玩家的距离
    public float cameraHeight = 2f;          // 摄像机高度
    public float followSpeed = 3f;           // 摄像机平滑跟随的速度

    private float yaw = 0f;                  // 水平旋转
    private float pitch = 0f;                // 垂直旋转
    private bool isCursorVisible = false;    // 控制鼠标是否可见
    private NetworkObject networkObject;     // 用于检查是否为本地拥有的对象

    private void Start()
    {
        // 获取 NetworkObject 组件
        networkObject = player.GetComponent<NetworkObject>();

        // 确保只在本地拥有的玩家端锁定光标并初始化相机
        if (networkObject != null && networkObject.IsOwner)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            // 如果不是本地玩家，销毁相机，避免多个客户端的相机冲突
            Destroy(gameObject);
        }
    }

    private void LateUpdate()
    {
        // 确保相机只跟随本地拥有的玩家
        if (networkObject != null && networkObject.IsOwner)
        {
            // 如果鼠标隐藏，才允许旋转相机
            if (!isCursorVisible)
            {
                RotateCamera();
            }
            UpdateCameraPosition();

            // 检查是否按下 O 键来切换鼠标的可见性
            if (Input.GetKeyDown(KeyCode.O))
            {
                ToggleCursorVisibility();
            }
        }
    }

    private void RotateCamera()
    {
        // 获取鼠标的水平和垂直移动
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 更新水平和垂直旋转角度
        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -30f, 60f); // 限制垂直角度，防止摄像机翻转
    }

    private void UpdateCameraPosition()
    {
        // 计算目标摄像机位置和旋转
        Vector3 cameraOffset = new Vector3(0f, cameraHeight, -distanceFromPlayer);
        Vector3 rotatedOffset = Quaternion.Euler(pitch, yaw, 0f) * cameraOffset;

        // 直接设置摄像机的位置到目标位置
        transform.position = player.position + rotatedOffset;

        // 设置摄像机朝向玩家的头部
        transform.LookAt(player.position + Vector3.up * cameraHeight);
    }

    private void ToggleCursorVisibility()
    {
        isCursorVisible = !isCursorVisible; // 切换鼠标的可见状态
        Cursor.visible = isCursorVisible;
        Cursor.lockState = isCursorVisible ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public Quaternion GetCameraRotation()
    {
        // 返回相机的水平旋转角度
        return Quaternion.Euler(0, yaw, 0);
    }
}