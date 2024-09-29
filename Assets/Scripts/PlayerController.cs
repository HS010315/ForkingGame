using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float jumpForce = 10f;        // 점프할 때의 힘
    public float rotationSpeed = 100f;   // 회전 속도
    public Rigidbody2D rb;
    public bool isStuck = false;         // 벽에 고정 여부
    public float slipSpeed = 0.5f;       // 미끄러짐 속도
    public float unstuckThresholdAngle = 30f; // 포크가 빠지는 임계 각도
    private Transform stuckWall;         // 고정된 벽 위치
    private float currentStickTime = 0f;

    void Update()
    {
        if (isStuck)
        {
            HandleStuckState();
        }
        else
        {
            RotateInAir();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isStuck)
            {
                Jump();
            }
        }
    }

    void Jump()
    {
        if (isStuck)
        {
            UnstickFromWall();  // 박힌 상태에서 점프
        }
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);  // 점프 처리
    }

    void RotateInAir()
    {
        float rotationInput = Input.GetAxis("Horizontal");
        rb.angularVelocity = -rotationInput * rotationSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            StickToWall(collision.transform);
        }
    }

    void StickToWall(Transform wall)
    {
        isStuck = true;
        stuckWall = wall;
        rb.velocity = Vector2.zero; // 속도를 초기화
        currentStickTime = 0f;      // 박힘 시간 초기화
    }

    void HandleStuckState()
    {
        currentStickTime += Time.deltaTime;

        // 미끄러운 벽에서 미끄러지기
        if (stuckWall.CompareTag("Slippery"))
        {
            rb.velocity = new Vector2(0, -slipSpeed);
        }

        // 벽의 기울기를 계산하여 포크가 빠지게 함
        Vector2 wallNormal = stuckWall.up;
        float wallAngle = Vector2.Angle(Vector2.up, wallNormal);
        if (wallAngle > unstuckThresholdAngle)
        {
            UnstickFromWall();
        }

        RotateWhileStuck();  // 고정 상태에서 회전 가능
    }

    void RotateWhileStuck()
    {
        float rotationInput = Input.GetAxis("Horizontal");
        transform.Rotate(0, 0, -rotationInput * rotationSpeed * Time.deltaTime);
    }

    void UnstickFromWall()
    {
        isStuck = false;
        stuckWall = null;
    }
}