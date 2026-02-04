using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BonusData
{
    public float percentBonus;
    public float fixedBonus;
    public float continuousBonus;

    public BonusData(float percent = 0, float fixedVal = 0, float continuous = 0)
    {
        percentBonus = percent;
        fixedBonus = fixedVal;
        continuousBonus = continuous;
    }
}