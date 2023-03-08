using Floyd_Warshall_Model;
using Floyd_Warshall.ViewModel.GraphComponents;
using System;
using System.Linq;

namespace Floyd_Warshall.ViewModel.Commands
{
    public class EdgeRightClickCommand : CommandBase
    {
        private GraphCanvasViewModel _vm;
        private GraphModel _graphModel;

        public EdgeRightClickCommand(GraphCanvasViewModel graphCanvasViewModel, GraphModel graphModel)
        {
            _vm = graphCanvasViewModel;
            _graphModel = graphModel;
        }

        public override void Execute(object? parameter)
        {
            int id = Convert.ToInt32(parameter);

            EdgeViewModelBase e = _vm.Edges.Single(e => e.Id == id);

            if(_vm.SelectedEdge == e)
            {
                _vm.SelectedEdge = null;
            }

            _graphModel.RemoveEdge(e.From.Vertex, e.To.Vertex);
            e.From.Edges.Remove(e);
            e.To.Edges.Remove(e);
            _vm.Edges.Remove(e);
            _vm.Views.Remove(e);
        }
    }
}
