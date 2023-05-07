using Floyd_Warshall.ViewModel.GraphComponents;
using System;
using System.Linq;
using Floyd_Warshall_Model.Model;

namespace Floyd_Warshall.ViewModel.Commands.GraphCanvasCommands
{
    /// <summary>
    /// Type of the vertex right click command
    /// </summary>
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

            foreach (EdgeViewModelBase e in v.Edges)
            {
                _viewModel.Edges.Remove(e);
                _viewModel.GraphComponents.Remove(e);
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
            _viewModel.GraphComponents.Remove(v);
        }
    }
}
