using Floyd_Warshall_Model;
using Floyd_Warshall_Model.Graph;
using Floyd_Warshall.ViewModel.GraphComponents;
using System.ComponentModel;

namespace Floyd_Warshall.ViewModel.Commands.GraphCanvasCommands
{
    public class CanvasClickCommand : GraphCanvasCommandBase
    {
        private readonly GraphModel _graphModel;

        public CanvasClickCommand(GraphCanvasViewModel viewModel, GraphModel graphModel) : base(viewModel)
        {
            _graphModel = graphModel;
        }

        public override void Execute(object? parameter)
        {
            Vertex v = new Vertex(_viewModel.GetVertexId);

            VertexViewModel vertex = new VertexViewModel(v)
            {
                CanvasX = _viewModel.MouseX - VertexViewModel.Size / 2,
                CanvasY = _viewModel.MouseY - VertexViewModel.Size / 2,
                IsSelected = false,
                RightClickCommand = new VertexRightClickCommand(_viewModel, _graphModel),
                LeftClickCommand = new VertexLeftClickCommand(_viewModel, _graphModel),
            };

            _graphModel.AddVertex(v);
            _viewModel.Verteces.Add(vertex);
            _viewModel.Views.Add(vertex);
        }

        public override bool CanExecute(object? parameter)
        {
            return !_viewModel.MaxVertexCountReached && base.CanExecute(parameter);
        }

        protected override void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(GraphCanvasViewModel.MaxVertexCountReached))
            {
                OnCanExecuteChanged();
            }
        }
    }
}
