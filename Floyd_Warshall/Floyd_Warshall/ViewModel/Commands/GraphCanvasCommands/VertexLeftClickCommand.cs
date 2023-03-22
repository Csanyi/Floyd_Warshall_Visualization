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
                v.IsSelected = true;
                _viewModel.SelectedVertex = v;
            }
            else if (_viewModel.SelectedVertex == v)
            {
                v.IsSelected = false;
                _viewModel.SelectedVertex = null;
            }
            else
            {
                if (_graphModel.IsDirected)
                {
                    AddDirectedEdge(_viewModel.SelectedVertex, v);
                }
                else
                {
                    AddUndirectedEdge(_viewModel.SelectedVertex, v);
                }

                _viewModel.SelectedVertex.IsSelected = false;
                _viewModel.SelectedVertex = null;
            }
        }

        private void AddUndirectedEdge(VertexViewModel from, VertexViewModel to)
        {
            Vertex f = from.Vertex;
            Vertex t = to.Vertex;

            if (_graphModel.GetEdge(f, t) == null)
            {
                _graphModel.AddEdge(f, t, 1);

                EdgeViewModel edge = new EdgeViewModel(_viewModel.GetEdgeId, _graphModel, from, to)
                {
                    Weight = 1,
                    IsSelected = false,
                    LeftClickCommand = new EdgeLeftClickCommand(_viewModel),
                    RightClickCommand = new EdgeRightClickCommand(_viewModel, _graphModel),
                };

                _viewModel.Edges.Add(edge);
                _viewModel.Views.Add(edge);
                from.Edges.Add(edge);
                to.Edges.Add(edge);
            }
            else
            {
                from.Edges.Single(e => e.From == from && e.To == to || e.To == from && e.From == to).Weight++;
            }
        }

        private void AddDirectedEdge(VertexViewModel from, VertexViewModel to)
        {
            Vertex f = from.Vertex;
            Vertex t = to.Vertex;

            if (_graphModel.GetEdge(f, t) == null)
            {
                _graphModel.AddEdge(f, t, 1);

                DirectedEdgeViewModel edge = new DirectedEdgeViewModel(_viewModel.GetEdgeId, _graphModel, from, to)
                {
                    Weight = 1,
                    IsSelected = false,
                    LeftClickCommand = new EdgeLeftClickCommand(_viewModel),
                    RightClickCommand = new EdgeRightClickCommand(_viewModel, _graphModel),
                };

                _viewModel.Edges.Add(edge);
                _viewModel.Views.Add(edge);
                from.Edges.Add(edge);
                to.Edges.Add(edge);
            }
            else
            {
                from.Edges.Single(e => e.From == from && e.To == to).Weight++;
            }
        }
    }
}
