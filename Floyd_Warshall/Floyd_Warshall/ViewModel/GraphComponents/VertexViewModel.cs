using System.Collections.Generic;

namespace Floyd_Warshall.ViewModel.GraphComponents
{
    public class VertexViewModel : GraphComponentViewModelBase
    {
        public static int Size { get { return 32; } }

        public VertexViewModel(int id) : base(id)
        {
            Edges = new List<EdgeViewModelBase>();
        }

        public List<EdgeViewModelBase> Edges { get; set; }

        private bool _inNegCycle;
        public bool InNegCycle
        {
            get { return _inNegCycle; }
            set
            {
                _inNegCycle = value;
                OnPropertyChanged();
            }
        }

        private double _canvasX;
        public override double CanvasX { 
            get { return _canvasX - VertexViewModel.Size / 2; }
            set
            {
                _canvasX = value;
                OnPropertyChanged();
            }
        }

        private double _canvasY;
        public override double CanvasY
        {
            get { return _canvasY - VertexViewModel.Size / 2; }
            set
            {
                _canvasY = value;
                OnPropertyChanged();
            }
        }

        public double GetX()
        {
            return _canvasX;
        }

        public double GetY()
        {
            return _canvasY;
        }
    }
}
