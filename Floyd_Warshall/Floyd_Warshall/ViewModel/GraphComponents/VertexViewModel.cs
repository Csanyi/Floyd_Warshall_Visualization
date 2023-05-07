using System.Collections.Generic;

namespace Floyd_Warshall.ViewModel.GraphComponents
{
    /// <summary>
    /// Type of the vertex viewmodel
    /// </summary>
    public class VertexViewModel : GraphComponentViewModelBase
    {
        private bool _inNegCycle; // indicates whether the vertex is in negative cycle
        private double _canvasX;  // x coord on canvas 
        private double _canvasY;  // y coord on canvas

        /// <summary>
        /// Constructor of the vertex viewmodel
        /// </summary>
        /// <param name="id">The vertex id</param>
        public VertexViewModel(int id) : base(id)
        {
            Edges = new HashSet<EdgeViewModelBase>();
        }

        /// <summary>
        /// Gets the vertex size
        /// </summary>
        public static int Size { get { return 32; } }

        /// <summary>
        /// Gets or sets the vertex in and out edges
        /// </summary>
        public ICollection<EdgeViewModelBase> Edges { get; set; }

        /// <summary>
        /// Gets or sets the inNegCycle field
        /// </summary>
        public bool InNegCycle
        {
            get { return _inNegCycle; }
            set
            {
                _inNegCycle = value;
                OnPropertyChanged();
            }
        }

        public override double CanvasX { 
            get { return _canvasX - VertexViewModel.Size / 2; }
            set
            {
                _canvasX = value;
                OnPropertyChanged();
            }
        }

        public override double CanvasY
        {
            get { return _canvasY - VertexViewModel.Size / 2; }
            set
            {
                _canvasY = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the top left corners's x coord on the canvas
        /// </summary>
        public double GetX()
        {
            return _canvasX;
        }

        /// <summary>
        /// Gets the top left corner's y coord on the canvas
        /// </summary>
        public double GetY()
        {
            return _canvasY;
        }
    }
}
