using UnityEngine;
using Vant.MVC;

namespace Miner.GamePlay
{
    public enum NewMVCComponentEvent
    {
        PositionChanged
    }

    public class NewMVCComponentViewModel : AbstractViewModelBase
    {
        public BindableProperty<Vector3> Position { get; } = new BindableProperty<Vector3>(Vector3.zero);

        public override void BindProperties()
        {
            Position.Bind(OnPositionChanged);
        }

        public override void Destroy()
        {
            Position.UnbindAll();
        }

        private void OnPositionChanged(Vector3 position)
        {
            Notifier?.Dispatch(NewMVCComponentEvent.PositionChanged, position);
        }
    }
}
