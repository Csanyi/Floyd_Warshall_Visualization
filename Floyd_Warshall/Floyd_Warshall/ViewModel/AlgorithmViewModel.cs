using Floyd_Warshall.ViewModel.Commands.AlgorithmCommands;
using Floyd_Warshall_Model.Model;
using Floyd_Warshall_Model.Model.Algorithm;
using Floyd_Warshall_Model.Model.Events;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Threading;

namespace Floyd_Warshall.ViewModel
{
	/// <summary>
	/// Type of the algorithm viewmodel
	/// </summary>
	public class AlgorithmViewModel : ViewModelBase
    {
        #region Fields

        private readonly GraphModel _graphModel; // the graph model
        private readonly DispatcherTimer _timer; // the timer
        private int _size;                       // size of the graph (number of verteces)
        private bool _isStopped;                 // indicates whether the algorithm is stopped
        private bool _isNegCycleFound;           // indicates whether the algorithm is found a negative cycle
        private bool _steppedOnce;               // indicates whether the algorithm is stepped once

        public const int CriticalTime = 100;     // the view does not update in every step under critical time (ms)

        #endregion

        #region Properties

        /// <summary>
        /// Gets the timer
        /// </summary>
        public DispatcherTimer Timer { get { return _timer; } }

        /// <summary>
        /// Gets or sets the timer interval
        /// </summary>
        public int TimerInterval
        {
            get { return _timer.Interval.Seconds * 1000 + _timer.Interval.Milliseconds; }
            set
            {
                _timer.Interval = TimeSpan.FromMilliseconds(value);
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the size
        /// </summary>
        public int Size
        {
            get { return _size; }
            set
            {
                _size = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the vertex id being processed
        /// </summary>
        public int? K { get { return _graphModel.K; } }

        /// <summary>
        /// Gets the previous vertex id being processed
        /// </summary>
        public int? PrevK { get { return _graphModel.PrevK; } } 

        /// <summary>
        /// Indicates whether the vertex count is reached the minimum to start the algorithm
        /// </summary>
        public bool IsEnoughVerteces { get { return _graphModel.GetVertexCount() > 1; } }

        /// <summary>
        /// Gets or sets the isNegCycleFound field
        /// </summary>
        public bool IsNegCycleFound
        { 
            get { return _isNegCycleFound; }
            set
            {
                _isNegCycleFound = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the steppedOnce field
        /// </summary>
        public bool SteppedOnce 
        { 
            get { return _steppedOnce; }
            set
            {
                _steppedOnce = value;
                OnPropertyChanged();
            } 
        }

        /// <summary>
        /// Indicates whether the algorithm is initialized
        /// </summary>
        public bool IsInitialized { get { return _graphModel.IsAlgorthmInitialized; } }

        /// <summary>
        /// Incicates whether the algorithm has next step
        /// </summary>
        public bool HasNextStep { get { return _graphModel.HasNextStep; } }

        /// <summary>
        /// Incicates whether the algorithm has previous step
        /// </summary>
        public bool HasPreviousStep { get { return _graphModel.HasPreviousStep; } }

        /// <summary>
        /// Gets or sets the isStopped field
        /// </summary>
        public bool IsStopped
        {
            get => _isStopped;
            set
            {
                _isStopped = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<MatrixGridViewModel> D { get; private set; }
        public ObservableCollection<MatrixGridViewModel> Pi { get; private set; }
        public ObservableCollection<MatrixGridViewModel> PrevPi { get; private set; }
        public ObservableCollection<MatrixGridViewModel> PrevD { get; private set; }
        public ObservableCollection<int> VertexIds { get; private set; }

        public ICommand InitCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
        public ICommand PauseCommand { get; private set; }
        public ICommand StartCommand { get; private set; }
        public ICommand StepCommand { get; private set; }
        public ICommand StepBackCommand { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of the algorithm viewmodel
        /// </summary>
        /// <param name="graphModel">The graph model</param>
        public AlgorithmViewModel(GraphModel graphModel)
        {
            _graphModel = graphModel;

            InitCommand = new AlgorithmInitCommand(this, graphModel);
            CancelCommand = new AlgorithmCancelCommand(this, graphModel);

            PauseCommand = new AlgorithmPauseCommand(this);
            StartCommand = new AlgorithmStartCommand(this);

            StepCommand = new AlgorithmStepCommand(this, graphModel);
            StepBackCommand = new AlgorithmStepBackCommand(this, graphModel);

            _timer = new DispatcherTimer();
            TimerInterval = 1000;
            _timer.Tick += Timer_Tick;

            _graphModel.VertexAdded += Model_VertexAdded;
            _graphModel.VertexRemoved += Model_VertexRemoved;
            _graphModel.AlgorithmInitialized += Model_AlgorithmInitialized;
            _graphModel.AlgorithmStepped += Model_AlgorithmStepped;
            _graphModel.AlgorithmSteppedBack += Model_AlgorithmSteppedBack;
            _graphModel.AlgorithmEnded += Model_AlgorithmEnded;
            _graphModel.NegativeCycleFound += Model_NegativeCycleFound;
            _graphModel.GraphLoaded += Model_GraphLoaded;
            _graphModel.NewGraphCreated += Model_NewGraphCreated;

            D = new ObservableCollection<MatrixGridViewModel>();
            Pi = new ObservableCollection<MatrixGridViewModel>();
            PrevD = new ObservableCollection<MatrixGridViewModel>();
            PrevPi = new ObservableCollection<MatrixGridViewModel>();
            VertexIds = new ObservableCollection<int>();

            IsStopped = true;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Calls the OnPropertyChanged method
        /// </summary>
        /// <param name="propertyName">The changed property name</param>
        public void CallPropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }

        /// <summary>
        /// Updates the algorithm data
        /// </summary>
        public void UpdateData()
        {
            AlgorithmData? e = _graphModel.GetAlgorithmData();

            if (e != null)
            {
                for (int i = 0; i < D.Count; ++i)
                {
                    int j = i / e.D.GetLength(0);
                    int k = i % e.D.GetLength(1);
                    PrevD[i].Value = e.PrevD[j,k];
                    PrevPi[i].Value = e.PrevPi[j,k];
                    D[i].Value = e.D[j, k];
                    Pi[i].Value = e.Pi[j, k];

                    Change change = new Change(j,k);
                    bool changed = e.ChangesD.Contains(change);
                    PrevD[i].Changed = changed;
                    D[i].Changed = changed;

                    changed = e.ChangesPi.Contains(change);
                    PrevPi[i].Changed = changed;
                    Pi[i].Changed = changed;
                }
            }
        }

        #endregion

        #region Timer event handlers

        /// <summary>
        /// Timer tick event handler, steps the algorithm
        /// </summary>
        private void Timer_Tick(object? sender, EventArgs e)
        {
            _graphModel.StepAlgorithm();
        }

        #endregion

        #region Model event handlers

        /// <summary>
        /// Vertex added event handler
        /// </summary>
        private void Model_VertexAdded(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(IsEnoughVerteces));
        }

        /// <summary>
        /// Vertex removed event handler
        /// </summary>
        private void Model_VertexRemoved(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(IsEnoughVerteces));
        }

        /// <summary>
        /// Algorithm initialized event handler, sets the data of the matrices
        /// </summary>
        private void Model_AlgorithmInitialized(object? sender, AlgorithmInitEventArgs e)
        {
            OnPropertyChanged(nameof(K));
            OnPropertyChanged(nameof(PrevK));
            SteppedOnce = false;

            for (int i = 0; i < e.D.GetLength(0); ++i)
            {
                for(int j = 0; j < e.D.GetLength(1); ++j)
                {
                    int ind = i * e.D.GetLength(0) + j;

                    D.Add(new MatrixGridViewModel()
                    {
                        Index = ind,
                        Value = e.D[i, j],
                        X = VertexIds[i],
                        Y = VertexIds[j],
                        ClickCommand = new MatrixGridClickCommand(this, _graphModel),
                    });
                    Pi.Add(new MatrixGridViewModel()
                    {
                        Index = ind,
                        Value = e.Pi[i, j],
                        X = VertexIds[i],
                        Y = VertexIds[j],
                        ClickCommand = new MatrixGridClickCommand(this, _graphModel),
                    });

                    PrevD.Add(new MatrixGridViewModel()
                    {
                        Index = ind,
                        Value = e.D[i, j],
                        X = VertexIds[i],
                        Y = VertexIds[j],
                        ClickCommand = new MatrixGridClickCommand(this, _graphModel),
                    });

                    PrevPi.Add(new MatrixGridViewModel()
                    {
                        Index = ind,
                        Value = e.Pi[i, j],
                        X = VertexIds[i],
                        Y = VertexIds[j],
                        ClickCommand = new MatrixGridClickCommand(this, _graphModel),
                    });
                }
            }
        }
        
        /// <summary>
        /// Algorithm ended event handler, stops the timer, 
        /// updates the data of the matrices if necessary
        /// </summary>
        private void Model_AlgorithmEnded(object? sender, EventArgs e)
        {
            if (TimerInterval <= CriticalTime && !IsStopped)
            {
                UpdateData();
            }

            _timer.Stop();
            IsStopped = true;
        }

        /// <summary>
        /// Algorithm stepped event handler, updates the data of the matrices if necessary
        /// </summary>
        private void Model_AlgorithmStepped(object? sender, AlgorithmSteppedEventArgs e)
        {
            OnPropertyChanged(nameof(K));
            OnPropertyChanged(nameof(PrevK));
            OnPropertyChanged(nameof(HasNextStep));
            OnPropertyChanged(nameof(HasPreviousStep));

            SteppedOnce = true;

            if (TimerInterval > CriticalTime || IsStopped)
            {
                foreach(ChangeValue c in e.ChangePrevD)
                {
                    int ind = c.I * Size + c.J;
                    D[ind].Changed = false;
                    PrevD[ind].Changed = false;
                    PrevD[ind].Value = c.Value;
                }

                foreach (ChangeValue c in e.ChangeD)
                {
                    int ind = c.I * Size + c.J;
                    D[ind].Changed = true;
                    PrevD[ind].Changed = true;
                    D[ind].Value = c.Value;
                }

                foreach (ChangeValue c in e.ChangePrevPi)
                {
                    int ind = c.I * Size + c.J;
                    Pi[ind].Changed = false;
                    PrevPi[ind].Changed = false;
                    PrevPi[ind].Value = c.Value;
                }

                foreach (ChangeValue c in e.ChangePi)
                {
                    int ind = c.I * Size + c.J;
                    Pi[ind].Changed = true;
                    PrevPi[ind].Changed = true;
                    Pi[ind].Value = c.Value;
                }
            }
        }

        /// <summary>
        /// Algorithm stepped event handler, updates the data of the matrices if necessary
        /// </summary>
        private void Model_AlgorithmSteppedBack(object? sender, AlgorithmSteppedEventArgs e)
        {
            OnPropertyChanged(nameof(K));
            OnPropertyChanged(nameof(PrevK));
            OnPropertyChanged(nameof(HasNextStep));
            OnPropertyChanged(nameof(HasPreviousStep));

            IsNegCycleFound = false;

            if (TimerInterval > CriticalTime || IsStopped)
            {
                foreach (ChangeValue c in e.ChangeD)
                {
                    int ind = c.I * Size + c.J;
                    D[ind].Changed = false;
                    PrevD[ind].Changed = false;
                    D[ind].Value = c.Value;
                }

                foreach (ChangeValue c in e.ChangePrevD)
                {
                    int ind = c.I * Size + c.J;
                    D[ind].Changed = true;
                    PrevD[ind].Changed = true;
                    PrevD[ind].Value = c.Value;
                }

                foreach (ChangeValue c in e.ChangePi)
                {
                    int ind = c.I * Size + c.J;
                    Pi[ind].Changed = false;
                    PrevPi[ind].Changed = false;
                    Pi[ind].Value = c.Value;
                }

                foreach (ChangeValue c in e.ChangePrevPi)
                {
                    int ind = c.I * Size + c.J;
                    Pi[ind].Changed = true;
                    PrevPi[ind].Changed = true;
                    PrevPi[ind].Value = c.Value;
                }
            }
        }

        /// <summary>
        /// Negative cycle found event handler, stops the timer
        /// updates the data of the matrices if necessary
        /// </summary>
        private void Model_NegativeCycleFound(object? sender, EventArgs e)
        {
            if (TimerInterval <= CriticalTime && !IsStopped)
            {
                UpdateData();
            }

            _timer.Stop();
            IsStopped = true;

            IsNegCycleFound = true;
        }

        /// <summary>
        /// Graph loaded event handler
        /// </summary>
        private void Model_GraphLoaded(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(IsEnoughVerteces));
        }

        /// <summary>
        /// New graph created event handler
        /// </summary>
        private void Model_NewGraphCreated(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(IsEnoughVerteces));
        }

        #endregion
    }
}
