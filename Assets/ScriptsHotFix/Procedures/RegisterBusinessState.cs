using Vant.GamePlay.Procedure;
using Vant.System.FSM;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Vant.Core;
using Miner.UI;

namespace Miner.Business.Procedures
{
    public class RegisterBusinessState : ProcedureBase
    {
        public override void OnEnter()
        {
            Debug.Log("[游戏状态机] 进入注册业务模块状态");

            RegisterViews();

            ChangeState<GamePlayingState>();
        }

        public override void OnExit(bool isShutdown)
        {
            Debug.Log("[游戏状态机] 离开注册业务模块状态");
        }

        public void RegisterViews()
        {
            // 注册UI
            AppCore.Instance.UIManager.RegisterUIs(
                TestView.StaticConfig
            );
        }
    }
}