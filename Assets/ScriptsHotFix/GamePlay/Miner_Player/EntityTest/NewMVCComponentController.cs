using UnityEngine;
using Vant.MVC;

namespace Miner.GamePlay
{
    public class NewMVCComponentController : AbstractControllerBase<NewMVCComponentView, NewMVCComponentViewModel>
    {
        private Vector3 _targetPos;
        private float _speed = 2f;
        private float _range = 3f;

        #region Lifecycle

        /// <summary>
        /// 创建时调用 (只调用一次)
        /// </summary>
        protected override void OnInit()
        {
            _targetPos = GetRandomPosition();
            ViewModel.Position.Value = _targetPos;
        }

        /// <summary>
        /// 每帧刷新时调用
        /// </summary>
        protected override void OnUpdate(float deltaTime, float unscaledDeltaTime = 0)
        {
            if (ViewModel == null) return;

            Vector3 current = ViewModel.Position.Value;
            Vector3 next = Vector3.MoveTowards(current, _targetPos, _speed * deltaTime);
            ViewModel.Position.Value = next;

            if (Vector3.Distance(next, _targetPos) < 0.01f)
            {
                _targetPos = GetRandomPosition();
            }
        }

        /// <summary>
        /// 销毁时调用 (只调用一次)
        /// </summary>
        protected override void OnDestroy()
        {
            // no-op
        }

        private Vector3 GetRandomPosition()
        {
            Vector3 basePos = View?.gameObject != null ? View.gameObject.transform.position : Vector3.zero;
            float x = Random.Range(-_range, _range);
            float z = Random.Range(-_range, _range);
            return new Vector3(basePos.x + x, basePos.y, basePos.z + z);
        }

        #endregion
    }
}
