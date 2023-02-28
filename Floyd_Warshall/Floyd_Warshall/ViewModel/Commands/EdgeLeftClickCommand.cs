using Floyd_Warshall.ViewModel.GraphComponents;
using System;
using System.Linq;

namespace Floyd_Warshall.ViewModel.Commands
{
    public class EdgeLeftClickCommand : CommandBase
    {
        private GraphCanvasViewModel _vm;

        public EdgeLeftClickCommand(GraphCanvasViewModel graphCanvasViewModel)
        {
            _vm = graphCanvasViewModel;
        }

        public override void Execute(object parameter)
        {
            int id = Convert.ToInt32(parameter);

            EdgeViewModelBase e = _vm.Edges.Single(e => e.Id == id);

            if (_vm.SelectedEdge == e)
            {
                e.IsSelected = false;
                _vm.SelectedEdge = null;
            } else
            {
                if (_vm.SelectedEdge != null)
                {
                    _vm.SelectedEdge.IsSelected = false;
                }
                e.IsSelected = true;
                _vm.SelectedEdge = e;
            }
        }
    }
}
