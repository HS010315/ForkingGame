using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float jumpForce = 10f;        // ������ ���� ��
    public float rotationSpeed = 100f;   // ȸ�� �ӵ�
    public Rigidbody2D rb;
    public bool isStuck = false;         // ���� ���� ����
    public float slipSpeed = 0.5f;       // �̲����� �ӵ�
    public float unstuckThresholdAngle = 30f; // ��ũ�� ������ �Ӱ� ����
    private Transform stuckWall;         // ������ �� ��ġ
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
            UnstickFromWall();  // ���� ���¿��� ����
        }
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);  // ���� ó��
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
        rb.velocity = Vector2.zero; // �ӵ��� �ʱ�ȭ
        currentStickTime = 0f;      // ���� �ð� �ʱ�ȭ
    }

    void HandleStuckState()
    {
        currentStickTime += Time.deltaTime;

        // �̲����� ������ �̲�������
        if (stuckWall.CompareTag("Slippery"))
        {
            rb.velocity = new Vector2(0, -slipSpeed);
        }

        // ���� ���⸦ ����Ͽ� ��ũ�� ������ ��
        Vector2 wallNormal = stuckWall.up;
        float wallAngle = Vector2.Angle(Vector2.up, wallNormal);
        if (wallAngle > unstuckThresholdAngle)
        {
            UnstickFromWall();
        }

        RotateWhileStuck();  // ���� ���¿��� ȸ�� ����
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