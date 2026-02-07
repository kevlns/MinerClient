using Cysharp.Threading.Tasks;
using Vant.MVC;

namespace Miner.GamePlay
{
    public class NewMVCComponent : AbstractMVCEntityBase<NewMVCComponentController, NewMVCComponentView, NewMVCComponentViewModel>
    {
        public NewMVCComponent(NewMVCComponentController controller, NewMVCComponentView view, NewMVCComponentViewModel viewModel, bool showOnInit) : base(controller, view, viewModel, showOnInit)
        {
        }
    }
}
