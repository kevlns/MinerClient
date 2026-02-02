using Miner.UI;
using UnityEngine;
using Vant.Core;
using Vant.GamePlay.Procedure;
using Vant.System.FSM;
using Vant.UI.UIFramework;

namespace Miner.Business.Procedures
{
    public class GamePlayingState : ProcedureBase
    {
        public override void OnEnter()
        {
            Debug.Log("[游戏状态机] 进入游戏主循环状态");

            AppCore.Instance.Notifier.Dispatch(UIInternalEvent.OPEN_UI, UIName.TEST_VIEW);
        }

        public override void OnExit(bool isShutdown)
        {
            Debug.Log("[游戏状态机] 离开游戏主循环状态");
        }

        public override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {

        }

        public override void OnDestroy()
        {

        }
    }
}