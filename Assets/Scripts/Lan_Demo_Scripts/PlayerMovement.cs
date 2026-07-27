using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

[RequireComponent(typeof(OwnerNetworkAnimator))]
public class PlayerMovement : NetworkBehaviour
{
    public float moveSpeed = 5f;             // 移动速度
    public float jumpForce = 5f;             // 跳跃力
    public Transform cameraTransform;        // 相机的 Transform，用于获取摄像机方向
    public LayerMask groundLayer;            // 用于检测地面的层级
    public float rotationSpeed = 5f;         // 角色旋转速度

    private Rigidbody rb;
    private bool isGrounded;
    private Vector3 moveDirection;
    private Animator animator;
    private NetworkAnimator networkAnimator; // 网络动画同步器
  

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // 禁止旋转，以避免角色飞出场景

        animator = GetComponent<Animator>(); // 获取 Animator 组件
        networkAnimator = GetComponent<NetworkAnimator>(); // 获取 NetworkAnimator 组件

        if (animator == null)
        {
            Debug.LogError("Animator component not found on PlayerMovement object.");
        }

        // 确保每个玩家的相机只跟随自己
        if (IsOwner)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        if (IsServer && IsOwner)
        {
            // Host的初始位置
            transform.position = new Vector3(-4.67f, 0, 12.2f);
        }
        else if (IsClient && IsOwner)
        {
            // Client的初始位置
            transform.position = new Vector3(13.08f, 0, -10f);
        }
    }

    private void Update()
    {
        if (!IsOwner) return;

        CheckGroundStatus();
        HandleMovementInput();
        UpdateAnimation();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        HandleSpecialAnimations();
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        Move();
    }

    private void CheckGroundStatus()
    {
        float distanceToGround = GetComponent<Collider>().bounds.extents.y + 0.1f;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, distanceToGround, groundLayer);

        if (isGrounded && rb.velocity.y < 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, -2f, rb.velocity.z);
        }
    }

    private void HandleMovementInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 forward = cameraTransform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = cameraTransform.right;
        right.y = 0;
        right.Normalize();

        moveDirection = (forward * vertical + right * horizontal).normalized;
    }

    private void Move()
    {
        if (moveDirection.magnitude >= 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            Vector3 newPosition = rb.position + moveDirection * moveSpeed * Time.deltaTime;
            rb.MovePosition(newPosition);
        }
    }

    private void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void UpdateAnimation()
    {
        if (animator == null) return;
        float speed = moveDirection.magnitude * moveSpeed;
        animator.SetFloat("Speed", speed);
    }

    private void HandleSpecialAnimations()
    {
        if (IsOwner && IsClient)
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                animator.SetFloat("ActionType", 1f);
                networkAnimator.SetTrigger("Talk");
            }
            else if (Input.GetKeyDown(KeyCode.K))
            {
                animator.SetFloat("ActionType", 2f);
                networkAnimator.SetTrigger("Dance");
            }
            else if (Input.GetKeyDown(KeyCode.L))
            {
                animator.SetFloat("ActionType", 3f);
                networkAnimator.SetTrigger("Wave");
            }
            else if (moveDirection.magnitude < 0.1f)
            {
                animator.SetFloat("ActionType", 0f); // Idle 动作
            }
        }
    }
}