using Vant.GamePlay.Procedure;
using Vant.System.FSM;

namespace Miner.Business.Procedures
{
    public class RegisterModelAndViewsState : ProcedureBase
    {
        public override void OnEnter(IFsm<ProcedureManager> fsm)
        {
            RegsiterModel();
            RegsiterUI();
            // ChangeState<PlayerAgreementState>(fsm);
        }

        private void RegsiterUI()
        {
            // GameGlobal.Instance.AppCore.UIManager.RegisterUIs(
            //     PropDetailPanel.StaticConfig,
            //     GMPanel.StaticConfig,
            //     EditCharacterPanel.StaticConfig,
            //     CreateCharacterPanel.StaticConfig,
            //     PopupConfirm.StaticConfig,
            //     JoinChannelPanel.StaticConfig,
            //     HomePageInfoPanel.StaticConfig,
            //     PlayerAgreementPanel.StaticConfig,
            //     LoginPanel.StaticConfig,
            //     PopupWindow.StaticConfig,
            //     MainUIPanel.StaticConfig,
            //     PosterPanel.StaticConfig,
            //     PopupNotice.StaticConfig,
            //     PropTransferPanel.StaticConfig,
            //     SelectAvatarPanel.StaticConfig
            // );
        }

        private void RegsiterModel()
        {
            // var modelManager = GameGlobal.Instance.AppCore.ModelManager;

            // modelManager.RegisterModels(
            //     new HttpModel(),
            //     new WSModel(),
            //     new PlayerModel(),
            //     new ChannelModel()
            // );
        }

    }
}