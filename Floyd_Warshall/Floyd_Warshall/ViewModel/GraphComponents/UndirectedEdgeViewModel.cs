using Floyd_Warshall_Model.Model;
using Floyd_Warshall_Model.Model.Events;

namespace Floyd_Warshall.ViewModel.GraphComponents
{
	/// <summary>
	/// Type of the undirected edge viewmodel
	/// </summary>
	public class UndirectedEdgeViewModel : EdgeViewModelBase
    {
        /// <summary>
        /// Constructor of the undirected edge viewmodel
        /// </summary>
        /// <param name="id">The edge id</param>
        /// <param name="graphModel">The graph model</param>
        /// <param name="from">The start vertex viewmodel of the edge</param>
        /// <param name="to">The end vertex viewmodel of the edge></param>
        public UndirectedEdgeViewModel(int id, GraphModel graphModel, VertexViewModel from, VertexViewModel to) 
                                    : base(id, graphModel, from, to) { }

        public override double X1 { get { return From.CanvasX - CanvasX + VertexViewModel.Size / 2; } }

        public override double Y1 { get { return From.CanvasY - CanvasY + VertexViewModel.Size / 2; } }

        public override double X2 { get { return To.CanvasX - CanvasX + VertexViewModel.Size / 2; } }

        public override double Y2 { get { return To.CanvasY - CanvasY + VertexViewModel.Size / 2; } }

        protected override void Model_EdgeUpdated(object? sender, EdgeUpdatedEventArgs e)
        {
            if (e.From == From.Id && e.To == To.Id || e.From == To.Id && e.To == From.Id)
            {
                OnPropertyChanged(nameof(Weight));
                OnEdgeUpdated();
            }
        }
    }
}
