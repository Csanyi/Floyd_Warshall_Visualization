using System.Collections.Generic;
using Floyd_Warshall_Model.Graph;

namespace Floyd_Warshall.ViewModel.GraphComponents
{
    public class VertexViewModel : GraphComponentViewModel
    {
        public static int Size { get { return 32; } }

        public VertexViewModel(Vertex v) 
        {
            _vertex = v;
            Edges = new List<EdgeViewModelBase>();
        }

        private Vertex _vertex;
        public Vertex Vertex { get { return _vertex; } }

        public override int Id { get { return _vertex.Id; } }

        public List<EdgeViewModelBase> Edges { get; set; }

        private double _canvasX;
        public override double CanvasX { 
            get { return _canvasX; }
            set
            {
                _canvasX = value;
                OnPropertyChanged();
                foreach(EdgeViewModelBase e in Edges)
                {
                    e.LocationChanged();
                }
            }
        }

        private double _canvasY;
        public override double CanvasY
        {
            get { return _canvasY; }
            set
            {
                _canvasY = value;
                OnPropertyChanged();
                foreach (EdgeViewModelBase e in Edges)
                {
                    e.LocationChanged();
                }
            }
        }
    }
}
