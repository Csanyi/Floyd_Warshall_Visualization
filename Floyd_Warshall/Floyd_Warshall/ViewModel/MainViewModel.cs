using Floyd_Warshall.ViewModel.GraphComponents;
using Floyd_Warshall_Model;
using Floyd_Warshall_Model.Graph;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Floyd_Warshall.ViewModel
{
    using VertexLocation = Tuple<Vertex, double, double>;

    public class MainViewModel : ViewModelBase
    {
        private GraphCanvasViewModel _canvas;
        public GraphCanvasViewModel Canvas { get { return _canvas; } }

        public event EventHandler<bool> NewGraph;
        public event EventHandler LoadGraph;
        public event EventHandler<IEnumerable<VertexLocation>> SaveGraph;
        public event EventHandler Exit;
   
        public ICommand NewGraphCommand { get; private set; }
        public ICommand LoadGraphCommand { get; private set; }
        public ICommand SaveGraphCommand { get; private set; }
        public ICommand ExitCommand { get; private set; }

        public MainViewModel(GraphModel graphModel)
        { 
            _canvas = new GraphCanvasViewModel(graphModel);

            NewGraphCommand = new DelegateCommand(param => OnNewGraph((bool)param));
            LoadGraphCommand = new DelegateCommand(param => OnLoad());
            SaveGraphCommand = new DelegateCommand(param => OnSave());
            ExitCommand = new DelegateCommand(param => OnExit());
        }


        private void OnNewGraph(bool isDirected) => NewGraph?.Invoke(this, isDirected);

        private void OnLoad() => LoadGraph?.Invoke(this, EventArgs.Empty);

        private void OnSave() => SaveGraph?.Invoke(this, _canvas.GetLocations());

        private void OnExit() => Exit?.Invoke(this, EventArgs.Empty);
        
    }
}
