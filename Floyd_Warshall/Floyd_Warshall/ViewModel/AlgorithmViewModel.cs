using Floyd_Warshall.ViewModel.Commands.AlgorithmCommands;
using Floyd_Warshall_Model.Model;
using Floyd_Warshall_Model.Model.Algorithm;
using Floyd_Warshall_Model.Model.Events;
using System;
using System.Collections.ObjectModel;
using System.Threading.Channels;
using System.Windows.Input;
using System.Windows.Threading;

namespace Floyd_Warshall.ViewModel
{
    public class AlgorithmViewModel : ViewModelBase
    {
        #region Fields

        private readonly GraphModel _graphModel;
        private readonly DispatcherTimer _timer;
        private int _size;
        private bool _isStopped;
        private bool _isNegCycleFound;
        private bool _steppedOnce;

        public const int CriticalTime = 100;

        #endregion

        #region Properties

        public DispatcherTimer Timer { get { return _timer; } }

        public int TimerInterval
        {
            get { return _timer.Interval.Seconds * 1000 + _timer.Interval.Milliseconds; }
            set
            {
                _timer.Interval = TimeSpan.FromMilliseconds(value);
                OnPropertyChanged();
            }
        }

        public int Size
        {
            get { return _size; }
            set
            {
                _size = value;
                OnPropertyChanged();
            }
        }

        public int? K { get { return _graphModel.K; } }

        public int? PrevK { get { return _graphModel.PrevK; } } 

        public bool IsEnoughVerteces { get { return _graphModel.GetVertexCount() > 1; } }

        public bool IsNegCycleFound
        { 
            get { return _isNegCycleFound; }
            set
            {
                _isNegCycleFound = value;
                OnPropertyChanged();
            }
        }

        public bool SteppedOnce 
        { 
            get { return _steppedOnce; }
            set
            {
                _steppedOnce = value;
                OnPropertyChanged();
            } 
        }

        public bool IsInitialized { get { return _graphModel.IsAlgorthmInitialized; } }

        public bool IsRunning { get { return _graphModel.IsAlgorithmRunning; } }

        public bool IsStopped
        {
            get => _isStopped;
            set
            {
                _isStopped = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<MatrixGridViewModel> D { get; set; }
        public ObservableCollection<MatrixGridViewModel> Pi { get; set; }
        public ObservableCollection<MatrixGridViewModel> PrevPi { get; set; }
        public ObservableCollection<MatrixGridViewModel> PrevD { get; set; }
        public ObservableCollection<int> VertexIds { get; set; }

        public ICommand InitCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
        public ICommand PauseCommand { get; private set; }
        public ICommand StartCommand { get; private set; }
        public ICommand StepCommand { get; private set; }

        #endregion

        #region Constructors

        public AlgorithmViewModel(GraphModel graphModel)
        {
            _graphModel = graphModel;

            InitCommand = new AlgorithmInitCommand(this, graphModel);
            CancelCommand = new AlgorithmCancelCommand(this, graphModel);

            PauseCommand = new AlgorithmPauseCommand(this);
            StartCommand = new AlgorithmStartCommand(this);

            StepCommand = new AlgorithmStepCommand(this, graphModel);

            _timer = new DispatcherTimer();
            TimerInterval = 1000;
            _timer.Tick += Timer_Tick;

            _graphModel.VertexAdded += Model_VertexAdded;
            _graphModel.VertexRemoved += Model_VertexRemoved;
            _graphModel.AlgorithmInitialized += Model_AlgorithmInitialized;
            _graphModel.AlgorithmStepped += Model_AlgorithmStepped;
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

        public void CallPropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }

        public void UpdateData()
        {
            AlgorithmData? e = _graphModel.GetAlgorithmData();

            if (e != null)
            {

                for (int i = 0; i < D.Count; ++i)
                {
                    int j = i / e.D.GetLength(0);
                    int k = i % e.D.GetLength(1);
                    PrevD[i].Value = D[i].Value;
                    PrevPi[i].Value = Pi[i].Value;
                    D[i].Value = e.D[j, k];
                    Pi[i].Value = e.Pi[j, k];

                    bool changed = e.ChangesD != null && e.ChangesD.Contains(new Tuple<int, int>(j, k));
                    PrevD[i].Changed = changed;
                    D[i].Changed = changed;

                    changed = e.ChangesPi != null && e.ChangesPi.Contains(new Tuple<int, int>(j, k));
                    PrevPi[i].Changed = changed;
                    Pi[i].Changed = changed;
                }
            }
        }

        #endregion

        #region Timer event handlers

        private void Timer_Tick(object? sender, EventArgs e)
        {
            _graphModel.StepAlgorithm();
        }

        #endregion

        #region Model event handlers

        private void Model_VertexAdded(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(IsEnoughVerteces));
        }

        private void Model_VertexRemoved(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(IsEnoughVerteces));
        }

        private void Model_AlgorithmInitialized(object? sender, AlgorithmEventArgs e)
        {
            OnPropertyChanged(nameof(K));
            OnPropertyChanged(nameof(PrevK));
            SteppedOnce = false;

            for (int i = 0; i < e.Data.D.GetLength(0); ++i)
            {
                for(int j = 0; j < e.Data.D.GetLength(1); ++j)
                {
                    int ind = i * e.Data.D.GetLength(0) + j;

                    D.Add(new MatrixGridViewModel()
                    {
                        Index = ind,
                        Value = e.Data.D[i, j],
                        X = VertexIds[i],
                        Y = VertexIds[j],
                        ClickCommand = new MatrixGridClickCommand(this, _graphModel),
                    });
                    Pi.Add(new MatrixGridViewModel()
                    {
                        Index = ind,
                        Value = e.Data.Pi[i, j],
                        X = VertexIds[i],
                        Y = VertexIds[j],
                        ClickCommand = new MatrixGridClickCommand(this, _graphModel),
                    });

                    PrevD.Add(new MatrixGridViewModel()
                    {
                        Index = ind,
                        X = VertexIds[i],
                        Y = VertexIds[j],
                        ClickCommand = new MatrixGridClickCommand(this, _graphModel),
                    });

                    PrevPi.Add(new MatrixGridViewModel()
                    {
                        Index = ind,
                        X = VertexIds[i],
                        Y = VertexIds[j],
                        ClickCommand = new MatrixGridClickCommand(this, _graphModel),
                    });
                }
            }
        }
        
        private void Model_AlgorithmEnded(object? sender, EventArgs e)
        {
            if (TimerInterval <= CriticalTime && !IsStopped)
            {
                UpdateData();
            }

            _timer.Stop();
            IsStopped = true;

            OnPropertyChanged(nameof(IsRunning));
        }

        private void Model_AlgorithmStepped(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(K));
            OnPropertyChanged(nameof(PrevK));
            SteppedOnce = true;

            if (TimerInterval > CriticalTime || IsStopped)
            {
                UpdateData();
            }
        }

        private void Model_NegativeCycleFound(object? sender, EventArgs e)
        {
            IsNegCycleFound = true;
            OnPropertyChanged(nameof(IsRunning));
        }

        private void Model_GraphLoaded(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(IsEnoughVerteces));
        }

        private void Model_NewGraphCreated(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(IsEnoughVerteces));
        }

        #endregion
    }
}
