using Vant.GamePlay.Procedure;
using Vant.System.FSM;
using UnityEngine;
using Vant.Core;
using Miner.Config;
using Cysharp.Threading.Tasks;
using Luban;

namespace Miner.Business.Procedures
{
    public class LoadAndPreloadState : ProcedureBase
    {
        public override async void OnEnter()
        {
            Debug.Log("[游戏状态机] 进入加载与预加载状态");
            AppCore.GlobalSettings.LUBAN_HOTFIX = true;
            AppCore.GlobalSettings.LUBAN_CONFIG_PATH_HF = "Assets/LubanConfig/Bytes/";
            await AppCore.Instance.ConfigManager.LoadAsync(() => MinerTables.CreateAsync(LoadConfigFromAddressables));
        }

        public override void OnExit(bool isShutdown)
        {
            Debug.Log("[游戏状态机] 退出加载与预加载状态");
        }

        public override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            if (AppCore.Instance.ConfigManager.IsLoaded<MinerTables>())
            {
                ChangeState<RegisterBusinessState>();
            }
        }

        public override void OnDestroy()
        {

        }

        #region 自定义配置加载器

        /// <summary>
        /// 使用 Addressables 加载配置文件
        /// </summary>
        /// <param name="fileName">配置文件名（不含扩展名）</param>
        /// <returns>配置数据的 ByteBuf</returns>
        private async UniTask<ByteBuf> LoadConfigFromAddressables(string fileName)
        {
            try
            {
                string path = $"{AppCore.GlobalSettings.LUBAN_CONFIG_PATH_HF}{fileName}.bytes";
                var textAsset = await AppCore.Instance.MainAssetManager.LoadAssetAsync<TextAsset>(path);

                if (textAsset == null)
                {
                    Debug.LogError($"[加载策划表数据] 配置文件加载失败: {path}");
                    return new ByteBuf(new byte[0]);
                }

                var bytes = textAsset.bytes;
                AppCore.Instance.MainAssetManager.ReleaseAsset(path);
                return new ByteBuf(bytes);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[加载策划表数据] 加载配置文件异常: {fileName}, 错误: {ex.Message}");
                return new ByteBuf(new byte[0]);
            }
        }

        #endregion
    }
}