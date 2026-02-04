using UnityEngine;

public class Miner_PlayerMove : MonoBehaviour
{
    private Rigidbody2D rb;
    public float maxSpeed = 5f;
    public float acceleration = 20f;

    public float minX = -8f;
    public float maxX = 8f;

    private Miner_ModularHookController hookController;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        hookController = GameObject.FindWithTag("Hook").GetComponent<Miner_ModularHookController>();
    }

    void Update()
    {
        bool canMove = hookController != null &&
                      hookController.IsHookReady();

        if (canMove)
        {
            HandleMovement();
        }
        else
        {
            StopMovement();
        }
    }

    private void HandleMovement()
    {
        // 读取输入
        float input = 0f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) input -= 1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) input += 1f;

        // 计算目标速度
        float targetVelocityX = input * maxSpeed;

        // 平滑加速/减速
        float newVelocityX = Mathf.MoveTowards(rb.velocity.x, targetVelocityX, acceleration * Time.deltaTime);

        // 边界检查
        float predictedX = rb.position.x + newVelocityX * Time.deltaTime;
        if (predictedX < minX || predictedX > maxX)
        {
            newVelocityX = 0f;
            predictedX = Mathf.Clamp(predictedX, minX, maxX);
        }

        // 应用速度和位置
        rb.velocity = new Vector2(newVelocityX, rb.velocity.y);
        rb.position = new Vector2(predictedX, rb.position.y);
    }

    private void StopMovement()
    {
        rb.velocity = new Vector2(0f, rb.velocity.y);
    }
}