using Floyd_Warshall.ViewModel.Commands;
using Floyd_Warshall.ViewModel.GraphComponents;
using Floyd_Warshall_Model.Model;
using Floyd_Warshall_Model.Model.Events;
using System;
using System.Windows.Input;

namespace Floyd_Warshall.ViewModel
{
	/// <summary>
	/// Type of the main viewmodel
	/// </summary>
	public class MainViewModel : ViewModelBase
    {
        #region Fields

        private readonly GraphCanvasViewModel _graphCanvas; // graph canvas viewmodel
        private readonly AlgorithmViewModel _algorithm;     // FW algorithm viewmodel

        private bool _commandsEnabled;                      // indicates whether the commands are enabled

        #endregion

        #region Properties

        /// <summary>
        /// Gets the graph canvas viewmodel
        /// </summary>
        public GraphCanvasViewModel GraphCanvas { get { return _graphCanvas; } }

        /// <summary>
        /// Gets the FW algorithm viewmodel
        /// </summary>
        public AlgorithmViewModel Algorithm { get { return _algorithm; } }

        /// <summary>
        /// Gets or sets the commands enabled field
        /// </summary>
        public bool CommandsEnabled 
        {
            get { return _commandsEnabled; }
            set
            {
                _commandsEnabled = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// Gets the new graph command
        /// </summary>
        public ICommand NewGraphCommand { get; private set; }

        /// <summary>
        /// Gets the load graph command
        /// </summary>
        public ICommand LoadGraphCommand { get; private set; }

        /// <summary>
        /// Gets the save graph command
        /// </summary>
        public ICommand SaveGraphCommand { get; private set; }

        /// <summary>
        /// Gets the exit command
        /// </summary>
        public ICommand ExitCommand { get; private set; }

        /// <summary>
        /// Gets the language switch command
        /// </summary>
        public ICommand SwitchLanguageCommand { get; private set; }

        #endregion

        #region Events

        public event EventHandler<NewGraphEventArgs>? NewGraph;
        public event EventHandler? LoadGraph;
        public event EventHandler<GraphLocationEventArgs>? SaveGraph;
        public event EventHandler? Exit;
        public event EventHandler<LanguageEventArgs>? SwitchLanguage;

        #endregion

        #region Constructors

        /// <summary>
        /// Construtcor of the main viewmodel
        /// </summary>
        /// <param name="graphModel">The graph model</param>
        public MainViewModel(GraphModel graphModel)
        {
            _graphCanvas = new GraphCanvasViewModel(graphModel);
            _algorithm = new AlgorithmViewModel(graphModel);

            NewGraphCommand = new DelegateCommand(param => OnNewGraph(Convert.ToBoolean(param)), _ => CommandsEnabled);
            LoadGraphCommand = new DelegateCommand(_ => OnLoad(), _ => CommandsEnabled);
            SaveGraphCommand = new DelegateCommand(_ => OnSave());
            ExitCommand = new DelegateCommand(_ => OnExit());

			SwitchLanguageCommand = new DelegateCommand(param => OnLanguageSwitch(param as string));

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

        /// <summary>
        /// Triggers the NewGraph event
        /// </summary>
        /// <param name="isDirected">The direction of the new graph</param>
        private void OnNewGraph(bool isDirected) => NewGraph?.Invoke(this, new NewGraphEventArgs(isDirected));

        /// <summary>
        /// Triggers the LoadGraph event
        /// </summary>
        private void OnLoad() => LoadGraph?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Triggers the SaveGraph event
        /// </summary>
        private void OnSave() => SaveGraph?.Invoke(this, new GraphLocationEventArgs(_graphCanvas.GetLocations()));

        /// <summary>
        /// Triggers the Exit event
        /// </summary>
        private void OnExit() => Exit?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Triggers the SwitchLanguage event
        /// </summary>
        /// <param name="lang">The code of the language</param>
        private void OnLanguageSwitch(string? langCode) => SwitchLanguage?.Invoke(this, new LanguageEventArgs(langCode));

        #endregion
    }
}
