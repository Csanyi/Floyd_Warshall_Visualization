using Floyd_Warshall_Model;
using Floyd_Warshall_Model.Graph;
using Floyd_Warshall.ViewModel.GraphComponents;
using System;
using System.Linq;
using System.ComponentModel;

namespace Floyd_Warshall.ViewModel.Commands.GraphCanvasCommands
{
    public class VertexLeftClickCommand : GraphCanvasCommandBase
    {
        private readonly GraphModel _graphModel;

        public VertexLeftClickCommand(GraphCanvasViewModel viewModel, GraphModel graphModel) : base(viewModel)
        {
            _graphModel = graphModel;
        }

        public override void Execute(object? parameter)
        {
            int id = Convert.ToInt32(parameter);

            VertexViewModel v = _viewModel.Verteces.Single(v => v.Id == id);

            if (_viewModel.SelectedVertex == null)
            {
                _viewModel.SelectedVertex = v;
            }
            else if (_viewModel.SelectedVertex == v)
            {
                _viewModel.SelectedVertex = null;
            }
            else
            {
                if (!_graphModel.IsEdgeBetween(_viewModel.SelectedVertex.Id, v.Id))
                {
                    _graphModel.AddEdge(_viewModel.SelectedVertex.Id, v.Id, 1);
                }
                else
                {
                    _graphModel.IncrementWeight(_viewModel.SelectedVertex.Id, v.Id);
                }
                //else if (_graphModel.IsDirected)
                //{
                //    _viewModel.SelectedVertex.Edges.Single(e => e.From == _viewModel.SelectedVertex && e.To == v).Weight++;
                //}
                //else if (!_graphModel.IsDirected)
                //{
                //    _viewModel.SelectedVertex.Edges.Single(e => e.From == _viewModel.SelectedVertex && e.To == v 
                //                        || e.To == _viewModel.SelectedVertex && e.From == v).Weight++;
                //}

                _viewModel.SelectedVertex = null;
            }
        }
    }
}
