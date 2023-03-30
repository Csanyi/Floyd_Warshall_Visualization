using Floyd_Warshall_Model.Model;
using Floyd_Warshall_Model.Model.Events;
using System;
using System.ComponentModel;

namespace Floyd_Warshall.ViewModel.GraphComponents
{
    public abstract class EdgeViewModelBase : GraphComponentViewModel
    {
        private readonly GraphModel _graphModel;

        public event EventHandler? EdgeUpdated;

        protected EdgeViewModelBase(int id, GraphModel graphModel, VertexViewModel from, VertexViewModel to): base(id)
        { 
            _graphModel = graphModel;

            From = from;
            To = to;

            From.PropertyChanged += VertexPropertyChanged;
            To.PropertyChanged += VertexPropertyChanged;

            _graphModel.EdgeUpdated += Model_EdgeUpdated;
        }

        public VertexViewModel From { get; set; }
        public VertexViewModel To { get; set; }

        public short Weight
        {
            get { return _graphModel.GetWeight(From.Id, To.Id); }
        }

        public override double CanvasX { get { return Math.Min(From.CanvasX, To.CanvasX) + VertexViewModel.Size / 2; } }
        public override double CanvasY { get { return Math.Min(From.CanvasY, To.CanvasY) + VertexViewModel.Size / 2; } }

        public abstract double X1 { get; }
        public abstract double Y1 { get; }
        public abstract double X2 { get; }
        public abstract double Y2 { get; }

        protected virtual void VertexPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(VertexViewModel.CanvasX) || e.PropertyName == nameof(VertexViewModel.CanvasY))
            {
                OnPropertyChanged(nameof(CanvasX));
                OnPropertyChanged(nameof(CanvasY));
                OnPropertyChanged(nameof(X1));
                OnPropertyChanged(nameof(Y1));
                OnPropertyChanged(nameof(X2));
                OnPropertyChanged(nameof(Y2));
            }
        }

        protected abstract void Model_EdgeUpdated(object? sender, EdgeUpdatedEventArgs e);

        protected void OnEdgeUpdated() => EdgeUpdated?.Invoke(this, EventArgs.Empty);
    }
}
