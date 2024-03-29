﻿using Floyd_Warshall.ViewModel.GraphComponents;
using System;
using System.Linq;

namespace Floyd_Warshall.ViewModel.Commands.GraphCanvasCommands
{
	/// <summary>
	/// Type of the edge left click command
	/// </summary>
	public class EdgeLeftClickCommand : GraphCanvasCommandBase
    {
        public EdgeLeftClickCommand(GraphCanvasViewModel viewModel) : base(viewModel) { }

        public override void Execute(object? parameter)
        {
            int id = Convert.ToInt32(parameter);

            EdgeViewModelBase e = _viewModel.Edges.Single(e => e.Id == id);

            if (_viewModel.SelectedEdge == e)
            {
                _viewModel.SelectedEdge = null;
            }
            else
            {
                _viewModel.SelectedVertex = null;
                _viewModel.SelectedEdge = e;
            }
        }
    }
}
