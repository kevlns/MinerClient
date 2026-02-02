using Vant.GamePlay.Procedure;
using Vant.System.FSM;
using UnityEngine;
using Vant.Core;

namespace Miner.Business.Procedures
{
    public class LoadAndPreloadState : ProcedureBase
    {
        public override void OnInit(IFsm<ProcedureManager> fsm)
        {

        }

        public override void OnEnter(IFsm<ProcedureManager> fsm)
        {
            // AppCore.GlobalSettings.LUBAN_HOTFIX = false;
            // AppCore.GlobalSettings.LUBAN_CONFIG_PATH_NON_HF = "ConfigBinary/";
            // GameGlobal.Instance.AppCore.ConfigManager.Load<Tables>((loader) => new Tables(loader));
        }

        public override void OnExit(IFsm<ProcedureManager> fsm, bool isShutdown)
        {

        }

        public override void OnUpdate(IFsm<ProcedureManager> fsm, float elapseSeconds, float realElapseSeconds)
        {
            // if (GameGlobal.Instance.AppCore.ConfigManager.IsLoaded<Tables>())
            // {
            //     ChangeState<MainGamePlayState>(fsm);
            // }
        }

        public override void OnDestroy(IFsm<ProcedureManager> fsm)
        {

        }
    }
}