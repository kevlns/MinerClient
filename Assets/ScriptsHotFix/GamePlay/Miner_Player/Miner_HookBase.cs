using UnityEngine;

[System.Serializable]
public class HookStats
{
    public string hookName;
    public Sprite icon;
    public float hookSize = 0.5f;

    [Header("基础属性")]
    public float baseShootSpeed = 10f;
    public float baseReturnSpeed = 5f;
    public float baseRotateSpeed = 60f;
    public float maxDistance = 10f;
    public float hookPower = 1f; // 钩子力量，影响重量承受

    [Header("特殊能力")]
    public bool hasMagnetic = false;
    public float magneticRadius = 3f;
    public bool hasPierce = false;
    public bool hasFreezeResist = false;
    public bool hasSwampResist = false;

    [Header("升级系统")]
    public int level = 1;
    public int maxLevel = 5;
    public float[] levelMultipliers = { 1f, 1.2f, 1.4f, 1.6f, 1.8f, 2f };

    public float GetShootSpeed()
    {
        return baseShootSpeed * levelMultipliers[level - 1];
    }

    public float GetReturnSpeed()
    {
        return baseReturnSpeed * levelMultipliers[level - 1];
    }

    public float GetMaxDistance()
    {
        return maxDistance * levelMultipliers[level - 1];
    }

    public float GetHookPower()
    {
        return hookPower * levelMultipliers[level - 1];
    }
}

[CreateAssetMenu(fileName = "NewHook", menuName = "黄金矿工/钩子配置")]
public class Miner_HookBase : ScriptableObject
{
    public HookStats stats;

    // 钩子特殊能力接口
    public virtual void OnHookStart(Miner_ModularHookController controller) { }
    public virtual void OnHookUpdate(Miner_ModularHookController controller) { }
    public virtual void OnHookReturn(Miner_ModularHookController controller) { }
    public virtual void OnHookAttach(GameObject hookedObject, Miner_ModularHookController controller) { }
    public virtual void OnHookDetach(Miner_ModularHookController controller) { }
    public virtual void ApplyModifiers(Miner_ModularHookController controller) { }

    // 升级钩子
    public virtual bool UpgradeHook()
    {
        if (stats.level < stats.maxLevel)
        {
            stats.level++;
            return true;
        }
        return false;
    }
}

// 具体钩子类型示例
[CreateAssetMenu(fileName = "BasicHook", menuName = "黄金矿工/基础钩子")]
public class BasicHook : Miner_HookBase
{
    // 基础钩子没有特殊能力
}

[CreateAssetMenu(fileName = "MagneticHook", menuName = "黄金矿工/磁力钩子")]
public class MagneticHook : Miner_HookBase
{
    public override void OnHookStart(Miner_ModularHookController controller)
    {
        base.OnHookStart(controller);
    }

    public override void ApplyModifiers(Miner_ModularHookController controller)
    {
    }
}

[CreateAssetMenu(fileName = "PierceHook", menuName = "黄金矿工/穿刺钩子")]
public class PierceHook : Miner_HookBase
{
    public override void OnHookStart(Miner_ModularHookController controller)
    {
        base.OnHookStart(controller);
        // 穿刺钩子可以穿透脆弱物体
    }

    public override void OnHookAttach(GameObject hookedObject, Miner_ModularHookController controller)
    {

    }
}