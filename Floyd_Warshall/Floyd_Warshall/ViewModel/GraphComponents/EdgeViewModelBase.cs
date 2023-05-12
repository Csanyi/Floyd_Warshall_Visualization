using Floyd_Warshall_Model.Model;
using Floyd_Warshall_Model.Model.Events;
using System;
using System.ComponentModel;

namespace Floyd_Warshall.ViewModel.GraphComponents
{
	/// <summary>
	/// Type of the edge viewmodel base class
	/// </summary>
	public abstract class EdgeViewModelBase : GraphComponentViewModelBase
    {
        private readonly GraphModel _graphModel; // graph model

        /// <summary>
        /// Edge updated event
        /// </summary>
        public event EventHandler? EdgeUpdated;

        /// <summary>
        /// Constructor of the edge viewmodel base class
        /// </summary>
        /// <param name="id">The edge id</param>
        /// <param name="graphModel">The graph model</param>
        /// <param name="from">The start vertex viewmodel of the edge</param>
        /// <param name="to">The end vertex viewmodel of the edge</param>
        protected EdgeViewModelBase(int id, GraphModel graphModel, VertexViewModel from, VertexViewModel to): base(id)
        { 
            _graphModel = graphModel;

            From = from;
            To = to;

            From.PropertyChanged += VertexPropertyChanged;
            To.PropertyChanged += VertexPropertyChanged;

            _graphModel.EdgeUpdated += Model_EdgeUpdated;
        }

        /// <summary>
        /// Gets the start vertex viewmodel
        /// </summary>
        public VertexViewModel From { get; }

        /// <summary>
        /// Gets the end vertex viewmodel
        /// </summary>
        public VertexViewModel To { get; }

        /// <summary>
        /// Gets the edge weight
        /// </summary>
        public short Weight
        {
            get { return _graphModel.GetWeight(From.Id, To.Id) ?? 0; }
        }

        public override double CanvasX { get { return Math.Min(From.CanvasX, To.CanvasX) + VertexViewModel.Size / 2; } }
        public override double CanvasY { get { return Math.Min(From.CanvasY, To.CanvasY) + VertexViewModel.Size / 2; } }

        /// <summary>
        /// Gets the edge start's x coord
        /// </summary>
        public abstract double X1 { get; }

        /// <summary>
        /// Gets the edge start's y coord
        /// </summary>
        public abstract double Y1 { get; }

        /// <summary>
        /// Gets the edge end's x coord
        /// </summary>
        public abstract double X2 { get; }

        /// <summary>
        /// Gets the edge end's y coord
        /// </summary>
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

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, From.Id, To.Id);
        }
    }
}
