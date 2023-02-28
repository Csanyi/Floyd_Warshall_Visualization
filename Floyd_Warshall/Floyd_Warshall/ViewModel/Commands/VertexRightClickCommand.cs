using Floyd_Warshall_Model;
using Floyd_Warshall.ViewModel.GraphComponents;
using System;
using System.Linq;

namespace Floyd_Warshall.ViewModel.Commands
{
    public class VertexRightClickCommand : CommandBase
    {
        private GraphCanvasViewModel _vm;
        private GraphModel _graphModel;

        public VertexRightClickCommand(GraphCanvasViewModel graphCanvasViewModel, GraphModel graphModel)
        {
            _vm = graphCanvasViewModel;
            _graphModel = graphModel;
        }

        public override void Execute(object parameter)
        {
            int id = Convert.ToInt32(parameter);

            VertexViewModel v = _vm.Verteces.Single(v => v.Id == id);
            _graphModel.Graph.RemoveVertex(v.Vertex);
            if (_vm.SelectedVertex == v)
            {
                _vm.SelectedVertex = null;
            }

            for (int i = 0; i < v.Edges.Count; ++i)
            {
                EdgeViewModelBase e = v.Edges[i];
                _vm.Edges.Remove(e);
                _vm.Views.Remove(e);
                if (v == e.From)
                {
                    e.To.Edges.Remove(e);
                } else
                {
                    e.From.Edges.Remove(e);
                }
            }

            _vm.Verteces.Remove(v);
            _vm.Views.Remove(v);
        }
    }
}
