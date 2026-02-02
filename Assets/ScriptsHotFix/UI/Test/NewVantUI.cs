using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using Vant.Core;
using Vant.MVC;
using Vant.UI.UIFramework;

namespace Miner.UI
{
    /// <summary>
    /// UI 打开事件参数定义
    /// </summary>
    public static partial class UIName
    {
        /// <summary>
        /// UI 名称常量（NewVantUI -> NEW_VANT_U_I）
        /// </summary>
        public const string NEW_VANT_U_I = "NewVantUI";
    }

    public class NewVantUI : AbstractUIBase
    {

        #region UI Configuration

        public override UIConfig RegisterConfig => StaticConfig;
        public static readonly UIConfig StaticConfig = new UIConfig
        {
            Name = "NewVantUI",
            AssetPath = "",
            UIClass = typeof(NewVantUI),
            Layer = UILayer.Normal,
            Mode = UIMode.Overlay,
            NeedMask = false,
            IsCacheable = true,
            AllowMultiInstance = false,
            EnterAnimation = null,
            ExitAnimation = null,
        };

        #endregion

        #region Lifecycle

        /// <summary>
        /// 1. 创建时调用 (只调用一次)
        /// 用于初始化组件引用、事件监听等
        /// </summary>
        protected override void OnCreate()
        {
            base.OnCreate();
        }

        /// <summary>
        /// 2. 打开前调用
        /// 用于重置状态、准备数据。支持异步。
        /// </summary>
        protected override async UniTask OnBeforeOpen(object args)
        {
            await base.OnBeforeOpen(args);
        }

        /// <summary>
        /// 3. 刷新时调用
        /// 用于将数据绑定到 UI 元素
        /// </summary>
        protected override void OnRefresh()
        {
            base.OnRefresh();
        }

        /// <summary>
        /// 4. 打开后调用 (动画播放完毕后)
        /// </summary>
        protected override async UniTask OnAfterOpen()
        {
            await base.OnAfterOpen();
        }

        /// <summary>
        /// 5. 关闭前调用
        /// </summary>
        protected override async UniTask OnBeforeClose()
        {
            await base.OnBeforeClose();
        }

        /// <summary>
        /// 7. 销毁时调用
        /// </summary>
        protected override void OnDestroyUI()
        {
            base.OnDestroyUI();
        }

        #endregion
    }
}
