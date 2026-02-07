using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Miner.GamePlay;
using UnityEngine;
using Vant.Core;

public class TestEntityGenerator : MonoBehaviour
{
    void Start()
    {
        AppCore.Instance.MVCEntityManager.CreateEntityAsync<NewMVCComponent>().Forget();
    }
}
