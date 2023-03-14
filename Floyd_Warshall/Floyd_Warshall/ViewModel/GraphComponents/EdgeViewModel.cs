using Floyd_Warshall_Model;

namespace Floyd_Warshall.ViewModel.GraphComponents
{
    public class EdgeViewModel : EdgeViewModelBase
    {
        public EdgeViewModel(int id, GraphModel graphModel, VertexViewModel from, VertexViewModel to) 
                                    : base(id, graphModel, from, to) { }

        public override double X1 { get { return From.CanvasX - CanvasX + VertexViewModel.Size / 2; } }

        public override double Y1 { get { return From.CanvasY - CanvasY + VertexViewModel.Size / 2; } }

        public override double X2 { get { return To.CanvasX - CanvasX + VertexViewModel.Size / 2; } }

        public override double Y2 { get { return To.CanvasY - CanvasY + VertexViewModel.Size / 2; } }
    }
}
