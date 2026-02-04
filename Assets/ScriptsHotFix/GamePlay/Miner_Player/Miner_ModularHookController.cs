using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HookState
{
    Idle,
    Shooting,
    Returning,
    Frozen,
    Stuck
}

public class Miner_ModularHookController : MonoBehaviour
{
    [Header("组件引用")]
    public LineRenderer lineRenderer;
    public Transform hookVisualTransform;
    public Transform hookAttachmentPoint;
    public Transform anchorTransform;
    public Transform carriedObjectsContainer;

    [Header("当前钩子")]
    [SerializeField] private Miner_HookBase currentHook;
    public List<Miner_HookBase> ownedHooks = new List<Miner_HookBase>();
    public int selectedHookIndex = 0;

    [Header("状态")]
    public HookState currentState = HookState.Idle;
    public float currentHookLength = 0f;
    public float totalWeight = 0f;
    public int accumulatedTime = 0;

    [Header("临时效果")]
    public float swampSlowMultiplier = 1f;
    public float speedBoostMultiplier = 1f;
    public float magneticMultiplier = 1f;
    public bool isFrozen = false;

    [Header("运行时计算值")]
    public float effectiveShootSpeed;
    public float effectiveReturnSpeed;
    public float effectiveRotateSpeed;
    public float effectiveMaxDistance;

    // 钩子目标位置
    private Vector3 targetPosition;
    private Quaternion returnRotation;
    private float rotationTime = 0f;
    private Coroutine freezeCoroutine;

    void Awake()
    {
        if (ownedHooks.Count == 0)
        {
            var basicHook = ScriptableObject.CreateInstance<BasicHook>();
            basicHook.stats = new HookStats
            {
                hookName = "基础钩子",
                baseShootSpeed = 10f,
                baseReturnSpeed = 5f,
                baseRotateSpeed = 60f,
                maxDistance = 10f,
                hookPower = 1f
            };
            ownedHooks.Add(basicHook);
        }

        SwitchToHook(selectedHookIndex);
    }

    void Start()
    {
        InitializeLineRenderer();
        currentHook.OnHookStart(this);
    }

    void Update()
    {

        UpdateHookState();
        UpdateLineRenderer();
        UpdateHookVisual();

        HandleInput();
        CalculateEffectiveStats();
    }

    void FixedUpdate()
    {
        if (currentHook.stats.hasMagnetic && currentState == HookState.Shooting)
        {
            ApplyMagneticEffect();
        }
    }

    // ==================== 钩子切换系统 ====================
    public bool SwitchToHook(int index)
    {
        if (index >= 0 && index < ownedHooks.Count && currentState == HookState.Idle)
        {
            currentHook = ownedHooks[index];
            selectedHookIndex = index;

            // 更新钩子外观
            UpdateHookAppearance();

            // 通知钩子切换
            currentHook.OnHookStart(this);

            Debug.Log($"切换到钩子: {currentHook.stats.hookName}");
            return true;
        }
        return false;
    }

    public bool SwitchToHook(Miner_HookBase hook)
    {
        if (ownedHooks.Contains(hook) && currentState == HookState.Idle)
        {
            currentHook = hook;
            selectedHookIndex = ownedHooks.IndexOf(hook);

            UpdateHookAppearance();
            currentHook.OnHookStart(this);

            Debug.Log($"切换到钩子: {currentHook.stats.hookName}");
            return true;
        }
        return false;
    }

    public bool AddHook(Miner_HookBase newHook)
    {
        if (!ownedHooks.Contains(newHook))
        {
            ownedHooks.Add(newHook);
            return true;
        }
        return false;
    }

    private void UpdateHookAppearance()
    {
        // 更新钩子精灵
        var spriteRenderer = hookVisualTransform.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = currentHook.stats.icon;
        }

        // 更新碰撞体大小
        var collider = hookVisualTransform.GetComponent<CircleCollider2D>();
        if (collider != null)
        {
            collider.radius = currentHook.stats.hookSize;
        }
    }

    // ==================== 钩子状态机 ====================
    private void UpdateHookState()
    {
        switch (currentState)
        {
            case HookState.Idle:
                UpdateIdleState();
                break;

            case HookState.Shooting:
                UpdateShootingState();
                break;

            case HookState.Returning:
                UpdateReturningState();
                break;

            case HookState.Frozen:
                // 冻结状态不更新
                break;

            case HookState.Stuck:
                UpdateStuckState();
                break;
        }

        // 钩子特殊能力更新
        currentHook.OnHookUpdate(this);
    }

    private void UpdateIdleState()
    {
        rotationTime += Time.deltaTime;
        float angle = Mathf.Sin(rotationTime * effectiveRotateSpeed * Mathf.Deg2Rad) * 70f;
        Vector3 dir = Quaternion.Euler(0, 0, angle) * Vector3.down;

        hookVisualTransform.position = anchorTransform.position + dir * currentHookLength;
        hookVisualTransform.rotation = Quaternion.Euler(0, 0, angle);
        returnRotation = hookVisualTransform.rotation;
    }

    private void UpdateShootingState()
    {
        hookVisualTransform.position = Vector3.MoveTowards(
            hookVisualTransform.position,
            targetPosition,
            effectiveShootSpeed * Time.deltaTime
        );

        float distanceToTarget = Vector3.Distance(hookVisualTransform.position, targetPosition);
        float distanceToAnchor = Vector3.Distance(anchorTransform.position, hookVisualTransform.position);

        if (distanceToTarget < 0.1f || distanceToAnchor >= effectiveMaxDistance)
        {
            StartReturning();
        }
    }

    private void UpdateReturningState()
    {
        Vector3 targetDir = returnRotation * Vector3.down;
        Vector3 returnTarget = anchorTransform.position + targetDir * currentHookLength;

        hookVisualTransform.position = Vector3.MoveTowards(
            hookVisualTransform.position,
            returnTarget,
            effectiveReturnSpeed * Time.deltaTime
        );

        if (Vector3.Distance(hookVisualTransform.position, returnTarget) < 0.01f)
        {
            CompleteReturn();
        }
    }

    private void UpdateStuckState()
    {
        // 钩子卡住时的特殊逻辑
        // 例如：需要玩家按按钮挣脱
    }

    // ==================== 钩子操作 ====================
    public void ShootHook(Vector3 target)
    {
        if (currentState != HookState.Idle && currentState != HookState.Frozen) return;

        targetPosition = target;
        currentState = HookState.Shooting;

        Debug.Log($"发射钩子: {currentHook.stats.hookName}");
    }

    private void StartReturning()
    {
        currentState = HookState.Returning;
        Debug.Log("开始返回");
    }

    private void CompleteReturn()
    {
        currentState = HookState.Idle;
        DisposeCarriedObjects();
        totalWeight = 0f;
        accumulatedTime = 0;

        currentHook.OnHookReturn(this);
    }

    // ==================== 物品处理系统 ====================
    private void DisposeCarriedObjects()
    {
        if (carriedObjectsContainer.childCount == 0) return;

        List<Transform> objectsToProcess = new List<Transform>();
        foreach (Transform child in carriedObjectsContainer)
        {
            objectsToProcess.Add(child);
        }

        foreach (var obj in objectsToProcess)
        {
            ProcessHookedObject(obj.gameObject);
            Destroy(obj.gameObject);
        }
    }

    private void ProcessHookedObject(GameObject hookedObject)
    {
        // 钩子特殊能力处理
        currentHook.OnHookAttach(hookedObject, this);
    }

    public void AttachObject(GameObject obj)
    {
        if (obj == null) return;

        obj.transform.SetParent(carriedObjectsContainer);

        var rigidbody = obj.GetComponent<Rigidbody2D>();
        if (rigidbody != null)
        {
            rigidbody.isKinematic = true;
        }

        // 如果是射击状态，开始返回
        if (currentState == HookState.Shooting)
        {
            StartReturning();
        }
    }

    public void DetachObjectImmediately(GameObject obj)
    {
        if (obj.transform.parent == carriedObjectsContainer)
        {
            obj.transform.SetParent(null);
            Destroy(obj);
            CalculateTotalWeight();
        }
    }

    // ==================== 工具方法 ====================
    private void CalculateTotalWeight()
    {
        totalWeight = 0f;
        foreach (Transform child in carriedObjectsContainer)
        {

        }
    }

    private void CalculateEffectiveStats()
    {
        // 应用钩子基础属性
        effectiveShootSpeed = currentHook.stats.GetShootSpeed();
        effectiveReturnSpeed = currentHook.stats.GetReturnSpeed();
        effectiveRotateSpeed = currentHook.stats.baseRotateSpeed;
        effectiveMaxDistance = currentHook.stats.GetMaxDistance();

        // 应用重量影响
        float weightFactor = 1 + totalWeight / (currentHook.stats.GetHookPower() * 50 + 1);
        effectiveShootSpeed = Mathf.Max(effectiveShootSpeed / weightFactor, 1f);
        effectiveReturnSpeed = Mathf.Max(effectiveReturnSpeed / (weightFactor * 5), 0.1f);

        // 应用环境效果
        effectiveShootSpeed *= (1f / swampSlowMultiplier) * speedBoostMultiplier;
        effectiveReturnSpeed *= (1f / swampSlowMultiplier) * speedBoostMultiplier;

        // 钩子特殊能力修改
        currentHook.ApplyModifiers(this);
    }

    private void ApplyMagneticEffect()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(hookVisualTransform.position,
            currentHook.stats.magneticRadius * magneticMultiplier);
    }

    private void InitializeLineRenderer()
    {
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.useWorldSpace = true;
        }
    }

    private void UpdateLineRenderer()
    {
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, anchorTransform.position);
            lineRenderer.SetPosition(1, hookVisualTransform.position);
        }
    }

    private void UpdateHookVisual()
    {
        // 根据状态更新钩子视觉
        // 例如：不同状态的旋转、颜色变化等
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0) && currentState == HookState.Idle)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0;
            ShootHook(mouseWorldPos);
        }
    }

    // ==================== 环境交互 ====================
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("hookable"))
        {
            AttachObject(other.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("swamp"))
        {
            swampSlowMultiplier = 1f;
        }
    }

    public void StartFreeze(float duration)
    {
        if (freezeCoroutine != null)
        {
            StopCoroutine(freezeCoroutine);
        }
        freezeCoroutine = StartCoroutine(FreezeCoroutine(duration));
    }

    IEnumerator FreezeCoroutine(float duration)
    {
        currentState = HookState.Frozen;
        yield return new WaitForSeconds(duration);
        currentState = HookState.Returning;
        freezeCoroutine = null;
    }

    // ==================== 公共接口 ====================
    public bool IsHookReady()
    {
        return currentState == HookState.Idle;
    }

    public Miner_HookBase GetCurrentHook()
    {
        return currentHook;
    }

    public HookState GetHookState()
    {
        return currentState;
    }

    public float GetTotalWeight()
    {
        return totalWeight;
    }

    public int GetCarriedObjectCount()
    {
        return carriedObjectsContainer.childCount;
    }
}