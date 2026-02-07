using UnityEngine;
using Cysharp.Threading.Tasks;
using Vant.MVC;

namespace Miner.GamePlay
{
    public class NewMVCComponentView : AbstractGeneralViewBase
    {
        public TestEntitySkin Skin { get; private set; }


        #region View Configuration

        public override GeneralViewConfig RegisterConfig => StaticConfig;
        public static readonly GeneralViewConfig StaticConfig = new GeneralViewConfig
        {
            Name = "NewMVCComponentView",
            PrefabPath = "Assets/ArtResources/Prefabs/Test/TestEntity.prefab",
        };

        #endregion

        #region Lifecycle

        /// <summary>
        /// 创建时调用 (只调用一次)
        /// 用于初始化组件引用、事件监听等
        /// </summary>
        protected override void OnCreate()
        {
            base.OnCreate();
            Skin = new TestEntitySkin(gameObject);
            Notifier?.AddListener<Vector3>(NewMVCComponentEvent.PositionChanged, OnPositionChanged);
        }

        /// <summary>
        /// 打开前调用
        /// 用于重置状态、准备数据。支持异步。
        /// </summary>
        protected override async UniTask OnBeforeShow(object args)
        {
            await base.OnBeforeShow(args);
        }

        /// <summary>
        /// 打开UI时立即刷新一次
        /// 用于将数据绑定到 UI 元素
        /// </summary>
        protected override void OnRefreshOnceOnOpen()
        {
            base.OnRefreshOnceOnOpen();
        }

        /// <summary>
        /// 打开后调用 (动画播放完毕后)
        /// </summary>
        protected override async UniTask OnAfterShow()
        {
            await base.OnAfterShow();
        }

        /// <summary>
        /// 关闭时调用
        /// </summary>
        protected override void OnHide()
        {
            base.OnHide();
        }

        /// <summary>
        /// 销毁时调用
        /// </summary>
        protected override void OnDestroy()
        {
            Notifier?.RemoveAllListeners(this);
            base.OnDestroy();
        }

        private void OnPositionChanged(Vector3 position)
        {
            if (gameObject != null)
            {
                gameObject.transform.position = position;
            }
        }

        #endregion
    }
}
