using Floyd_Warshall.ViewModel.Commands;
using Floyd_Warshall.ViewModel.GraphComponents;
using Floyd_Warshall_Model;
using Floyd_Warshall_Model.Events;
using Floyd_Warshall_Model.Graph;
using Floyd_Warshall_Model.Persistence;
using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Windows.Input;

namespace Floyd_Warshall.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly GraphCanvasViewModel _graphCanvas;
        public GraphCanvasViewModel GraphCanvas { get { return _graphCanvas; } }

        private bool _commandsEnabled;
        public bool CommandsEnabled 
        {
            get => _commandsEnabled;
            set
            {
                _commandsEnabled = value;
                OnPropertyChanged();
            }
        }

        private readonly AlgorithmViewModel _algorithm;
        public AlgorithmViewModel Algorithm { get { return _algorithm; } }

        public event EventHandler<bool>? NewGraph;
        public event EventHandler? LoadGraph;
        public event EventHandler<IEnumerable<VertexLocation>>? SaveGraph;
        public event EventHandler? Exit;
   
        public ICommand NewGraphCommand { get; private set; }
        public ICommand LoadGraphCommand { get; private set; }
        public ICommand SaveGraphCommand { get; private set; }
        public ICommand ExitCommand { get; private set; }

        public MainViewModel(GraphModel graphModel)
        {
            _graphCanvas = new GraphCanvasViewModel(graphModel);
            _algorithm = new AlgorithmViewModel(graphModel);

            CommandsEnabled = true;

            NewGraphCommand = new DelegateCommand(param => OnNewGraph((bool)param), param => CommandsEnabled);
            LoadGraphCommand = new DelegateCommand(param => OnLoad(), param => CommandsEnabled);
            SaveGraphCommand = new DelegateCommand(param => OnSave());
            ExitCommand = new DelegateCommand(param => OnExit());

            graphModel.AlgorithmStarted += new EventHandler<AlgorithmEventArgs>(Model_AlgorithmStarted);
            graphModel.AlgorithmStopped += new EventHandler(Model_AlgorithmStopped);
        }


        private void OnNewGraph(bool isDirected) => NewGraph?.Invoke(this, isDirected);

        private void OnLoad() => LoadGraph?.Invoke(this, EventArgs.Empty);

        private void OnSave() => SaveGraph?.Invoke(this, _graphCanvas.GetLocations());

        private void OnExit() => Exit?.Invoke(this, EventArgs.Empty);


        private void Model_AlgorithmStarted(object? sender, AlgorithmEventArgs e)
        {
            CommandsEnabled = false;
        }

        private void Model_AlgorithmStopped(object? sender, EventArgs e)
        {
            CommandsEnabled = true;
        }
    }
}
