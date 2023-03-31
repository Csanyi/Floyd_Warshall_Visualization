using Floyd_Warshall.ViewModel.Commands;
using Floyd_Warshall.ViewModel.GraphComponents;
using Floyd_Warshall_Model.Model;
using Floyd_Warshall_Model.Model.Events;
using System;
using System.Windows.Input;

namespace Floyd_Warshall.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region Fields

        private readonly GraphCanvasViewModel _graphCanvas;
        private readonly AlgorithmViewModel _algorithm;

        private bool _commandsEnabled;

        #endregion

        #region Properties

        public GraphCanvasViewModel GraphCanvas { get { return _graphCanvas; } }

        public AlgorithmViewModel Algorithm { get { return _algorithm; } }

        public bool CommandsEnabled 
        {
            get { return _commandsEnabled; }
            set
            {
                _commandsEnabled = value;
                OnPropertyChanged();
            }
        }
   
        public ICommand NewGraphCommand { get; private set; }
        public ICommand LoadGraphCommand { get; private set; }
        public ICommand SaveGraphCommand { get; private set; }
        public ICommand ExitCommand { get; private set; }

        #endregion

        #region Events

        public event EventHandler<bool>? NewGraph;
        public event EventHandler? LoadGraph;
        public event EventHandler<GraphLocationEventArgs>? SaveGraph;
        public event EventHandler? Exit;

        #endregion

        #region Constructors

        public MainViewModel(GraphModel graphModel)
        {
            _graphCanvas = new GraphCanvasViewModel(graphModel);
            _algorithm = new AlgorithmViewModel(graphModel);

            NewGraphCommand = new DelegateCommand(param => OnNewGraph(Convert.ToBoolean(param)), param => CommandsEnabled);
            LoadGraphCommand = new DelegateCommand(param => OnLoad(), param => CommandsEnabled);
            SaveGraphCommand = new DelegateCommand(param => OnSave());
            ExitCommand = new DelegateCommand(param => OnExit());

            graphModel.AlgorithmInitialized += Model_AlgorithmInitialized;
            graphModel.AlgorithmCancelled += Model_AlgorithmCancelled;

            _commandsEnabled = true;
        }

        #endregion

        #region Model event handlers

        private void Model_AlgorithmInitialized(object? sender, EventArgs e)
        {
            CommandsEnabled = false;
        }

        private void Model_AlgorithmCancelled(object? sender, EventArgs e)
        {
            CommandsEnabled = true;
        }

        #endregion

        #region Private event methods

        private void OnNewGraph(bool isDirected) => NewGraph?.Invoke(this, isDirected);

        private void OnLoad() => LoadGraph?.Invoke(this, EventArgs.Empty);

        private void OnSave() => SaveGraph?.Invoke(this, new GraphLocationEventArgs(_graphCanvas.GetLocations()));

        private void OnExit() => Exit?.Invoke(this, EventArgs.Empty);

        #endregion
    }
}
