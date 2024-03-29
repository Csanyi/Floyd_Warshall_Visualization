﻿using Floyd_Warshall.ViewModel.GraphComponents;
using System;
using System.Linq;
using Floyd_Warshall_Model.Model;

namespace Floyd_Warshall.ViewModel.Commands.GraphCanvasCommands
{
	/// <summary>
	/// Type of the edge right click command
	/// </summary>
	public class EdgeRightClickCommand : GraphCanvasCommandBase
    {
        private readonly GraphModel _graphModel;

        public EdgeRightClickCommand(GraphCanvasViewModel viewModel, GraphModel graphModel) : base(viewModel)
        {
            _graphModel = graphModel;
        }

        public override void Execute(object? parameter)
        {
            int id = Convert.ToInt32(parameter);

            EdgeViewModelBase e = _viewModel.Edges.Single(e => e.Id == id);

            if (_viewModel.SelectedEdge == e)
            {
                _viewModel.SelectedEdge = null;
            }

            _graphModel.RemoveEdge(e.From.Id, e.To.Id);
            e.From.Edges.Remove(e);
            e.To.Edges.Remove(e);
            _viewModel.Edges.Remove(e);
            _viewModel.GraphComponents.Remove(e);
        }
    }
}
