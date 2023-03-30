using Floyd_Warshall.ViewModel.GraphComponents;
using System;
using System.Linq;
using Floyd_Warshall_Model.Model;

namespace Floyd_Warshall.ViewModel.Commands.GraphCanvasCommands
{
    public class VertexRightClickCommand : GraphCanvasCommandBase
    {
        private readonly GraphModel _graphModel;

        public VertexRightClickCommand(GraphCanvasViewModel viewModel, GraphModel graphModel) : base(viewModel)
        {
            _graphModel = graphModel;
        }

        public override void Execute(object? parameter)
        {
            int id = Convert.ToInt32(parameter);

            VertexViewModel v = _viewModel.Verteces.Single(v => v.Id == id);
            _graphModel.RemoveVertex(v.Id);
            if (_viewModel.SelectedVertex == v)
            {
                    _viewModel.SelectedVertex = null;
            }

            for (int i = 0; i < v.Edges.Count; ++i)
            {
                EdgeViewModelBase e = v.Edges[i];
                _viewModel.Edges.Remove(e);
                _viewModel.Views.Remove(e);
                if (v == e.From)
                {
                    e.To.Edges.Remove(e);
                }
                else
                {
                    e.From.Edges.Remove(e);
                }
            }

            _viewModel.Verteces.Remove(v);
            _viewModel.Views.Remove(v);
        }

        public override bool CanExecute(object? parameter)
        {
            return _viewModel.CanvasEnabled;
        }
    }
}
