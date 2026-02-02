using Vant.GamePlay.Procedure;
using Vant.System.FSM;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Miner.Business.Procedures
{
    public class InitPackageState : ProcedureBase
    {
        public override void OnEnter()
        {
            Debug.Log("[游戏状态机] 进入初始化资源包状态");

            // TODO 检查资源包完整性，检查资源版本
            ChangeState<LoadAndPreloadState>();
        }

        public override void OnExit(bool isShutdown)
        {
            Debug.Log("[游戏状态机] 离开初始化资源包状态");
        }

        public override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {

        }

        public override void OnDestroy()
        {

        }
    }
}