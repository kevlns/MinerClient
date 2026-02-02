using System;
using System.Collections.Generic;
using UnityEngine;
using Vant.Core;
using Vant.Utils;
using Vant.GamePlay.Procedure;
using Miner.Business.Procedures;

namespace Miner.Business.Global
{
    public class GameGlobal : SingletonMono<GameGlobal>, IGameLifeCycle
    {
        [SerializeField] private Transform uiRoot;

        private AppCore _appCore;
        public AppCore AppCore => _appCore;

        public event Action<float, float> OnUpdateEvent;
        public event Action<float, float> OnLateUpdateEvent;

        public event Action<float> OnFixedUpdateEvent;
        public event Action<bool> OnApplicationFocusEvent;
        public event Action<bool> OnApplicationPauseEvent;
        public event Action OnApplicationQuitEvent;
        public event Action OnDestroyEvent;

        private void Start()
        {
            AppCore.GlobalSettings.UI_LRU_MAX_SIZE = 3;

            // 初始化 AppCore
            _appCore = new AppCore(this);

            // 扫描并注册 GM 命令
            _appCore.GMManager.ScanAndRegisterStaticMethods(this.GetType().Assembly);

            _appCore.UIManager.Init(uiRoot);

            // 配置全局 HTTP 头
            _appCore.NetManager.HttpClient.SetGlobalHeader("Content-Type", "application/json");

            // 注册游戏流程
            List<ProcedureBase> procedures = new List<ProcedureBase>
            {
                new InitPackageState(),
                new LoadAndPreloadState(),
                new RegisterModelAndViewsState(),
                new GamePlayingState(),
            };
            _appCore.ProcedureManager.Init(procedures.ToArray());

            // 启动初始流程
            _appCore.ProcedureManager.StartProcedure<InitPackageState>();
        }

        private void Update()
        {
            OnUpdateEvent?.Invoke(Time.deltaTime, Time.unscaledDeltaTime);
        }

        private void LateUpdate()
        {
            OnLateUpdateEvent?.Invoke(Time.deltaTime, Time.unscaledDeltaTime);
        }

        private void FixedUpdate()
        {
            OnFixedUpdateEvent?.Invoke(Time.fixedDeltaTime);
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            OnApplicationFocusEvent?.Invoke(hasFocus);
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            OnApplicationPauseEvent?.Invoke(pauseStatus);
        }

        protected override void OnApplicationQuit()
        {
            base.OnApplicationQuit();
            OnApplicationQuitEvent?.Invoke();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            OnDestroyEvent?.Invoke();
        }
    }
}