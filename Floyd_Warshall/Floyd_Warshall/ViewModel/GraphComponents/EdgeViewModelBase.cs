using Floyd_Warshall_Model.Graph;
using System;

namespace Floyd_Warshall.ViewModel.GraphComponents
{
    public abstract class EdgeViewModelBase : GraphComponentViewModel
    {
        private GraphBase _graph;

        protected EdgeViewModelBase(int id, GraphBase graph) { _id = id; _graph = graph;  }

        private int _id;
        public override int Id { get { return _id; } }

        public VertexViewModel From { get; set; }
        public VertexViewModel To { get; set; }

        public int Weight
        {
            get { return _graph.GetWeight(From.Vertex, To.Vertex); }
            set
            {
                _graph.UpdateWeight(From.Vertex, To.Vertex, value);
                OnPropertyChanged();
            }
        }

        public override double CanvasX { get { return Math.Min(From.CanvasX, To.CanvasX) + VertexViewModel.Size / 2; } }
        public override double CanvasY { get { return Math.Min(From.CanvasY, To.CanvasY) + VertexViewModel.Size / 2; } }

        public abstract double X1 { get; }
        public abstract double Y1 { get; }
        public abstract double X2 { get; }
        public abstract double Y2 { get; }

        public virtual void LocationChanged()
        {
            OnPropertyChanged(nameof(CanvasX));
            OnPropertyChanged(nameof(CanvasY));
            OnPropertyChanged(nameof(X1));
            OnPropertyChanged(nameof(Y1));
            OnPropertyChanged(nameof(X2));
            OnPropertyChanged(nameof(Y2));
        }
    }
}
